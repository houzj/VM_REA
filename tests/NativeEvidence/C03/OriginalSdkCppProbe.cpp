#include <cstdio>
#include <cstring>
#include <fstream>
#include <memory>
#include <stdexcept>
#include <string>
#include <vector>
#include <windows.h>
#include <tlhelp32.h>
#include "Algorithms/BlobFindCpp.h"
#include "Algorithms/GlobalInitCpp.h"
#include "Algorithms/PreproMaskCpp.h"
#include "MVDImageCpp.h"
#include "MVDShapeCpp.h"
#include "MVD_ImageParamDefine.h"

static bool CaptureLoadedModules(const char* outDir) {
  const HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, GetCurrentProcessId());
  if (snapshot == INVALID_HANDLE_VALUE) return false;
  MODULEENTRY32W module{};
  module.dwSize = sizeof(module);
  bool complete = false;
  std::ofstream manifest(std::string(outDir) + "/loaded-modules.txt", std::ios::binary);
  if (manifest && Module32FirstW(snapshot, &module)) {
    do {
      char path[MAX_PATH * 4];
      const int size = WideCharToMultiByte(CP_UTF8, WC_ERR_INVALID_CHARS, module.szExePath,
                                          -1, path, sizeof(path), nullptr, nullptr);
      if (size == 0) break;
      manifest.write(path, size - 1).put('\n');
      if (!manifest) break;
      if (!Module32NextW(snapshot, &module)) {
        complete = GetLastError() == ERROR_NO_MORE_FILES;
        break;
      }
    } while (true);
  }
  CloseHandle(snapshot);
  manifest.close();
  return complete && manifest.good();
}

static void CaptureOutputImage(const char* outDir, const char* fileName, const char* label,
                               VisionDesigner::IMvdImage* image) {
  if (!image) {
    std::printf("%s=null\n", label);
    return;
  }
  const auto* channel = image->GetImageData(0);
  if (!channel || !channel->pData || channel->nLen == 0) {
    std::printf("%s=empty\n", label);
    return;
  }
  std::ofstream file(std::string(outDir) + "/" + fileName, std::ios::binary);
  file.write(reinterpret_cast<const char*>(channel->pData), channel->nLen);
  file.close();
  if (!file) throw std::runtime_error("Cannot save output image");
  std::printf("%s=%u x %u fmt=%d\n", label, image->GetWidth(), image->GetHeight(),
              static_cast<int>(image->GetPixelFormat()));
  std::printf("%sDataSize=%u %sRowStep=%u\n", label, channel->nLen, label, channel->nRowStep);
}

int main(int argc, char** argv) {
  const char* file = argc > 1 ? argv[1] : "D:\\MyWork\\图片库\\标定\\1.jpg";
  const char* outDir = argc > 2 ? argv[2] : "analysis/full-audit/batch1-20261107/C03/native-sdk-run";
  const char* paramFile = argc > 3 ? argv[3] : nullptr;
  const bool fullRoiFromMemory = argc > 4 && std::strcmp(argv[4], "full-roi-memory") == 0;
  const bool fromMemory = fullRoiFromMemory || (argc > 4 && std::strcmp(argv[4], "default-memory") == 0);
  const bool explicitFullRoi = fullRoiFromMemory || (argc > 4 && std::strcmp(argv[4], "full-roi") == 0);
  const bool generatedFullRoi = argc > 4 && std::strcmp(argv[4], "generated-full-roi") == 0;
  const bool outsideMask = argc > 4 && std::strcmp(argv[4], "outside-mask") == 0;
  const bool insideMask = argc > 4 && std::strcmp(argv[4], "inside-mask") == 0;
  const std::string imageFlags = argc > 5 ? argv[5] : "11"; // BinaryImage, then BlobImage.
  if (imageFlags.size() != 2 || imageFlags.find_first_not_of("01") != std::string::npos) {
    std::fprintf(stderr, "Expected two output flags: BinaryImage then BlobImage\n");
    return 6;
  }
  void* global = nullptr;
  VisionDesigner::BlobFind::IBlobFindTool* tool = nullptr;
  VisionDesigner::IMvdImage* image = nullptr;
  VisionDesigner::IMvdRectangleF* roi = nullptr;
  VisionDesigner::IMvdPolygonF* mask = nullptr;
  // The SDK's memory initializer borrows these bytes for the lifetime of the image.
  std::vector<unsigned char> inputPixels;
  // Keep region inputs alive until both consumers finish.
  std::unique_ptr<VisionDesigner::IMvdRectangleF, decltype(&DestroyRectangleInstance)> generatedRegion(
      nullptr, DestroyRectangleInstance);
  std::unique_ptr<VisionDesigner::PreproMask::IPreproMaskTool, decltype(&DestroyPreproMaskToolInstance)> preproMask(
      nullptr, DestroyPreproMaskToolInstance);
  const auto release = [&] {
    if (tool) DestroyBlobFindToolInstance(tool);
    preproMask.reset();
    generatedRegion.reset();
    if (roi) DestroyRectangleInstance(roi);
    if (mask) DestroyPolygonInstance(mask);
    if (image) DestroyImageInstance(image);
    if (global) DestroyGlobalInitToolInstance(global);
  };
  int g=-1,c=-1;
  const char* stage = "initialize";
  try {
    g = CreateGlobalInitToolInstance(&global);
    c = CreateBlobFindToolInstance(&tool);
    std::printf("{\"global\":%d,\"create\":%d,\"file\":\"%s\"}\n",g,c,file);
    if(c!=0 || !tool) { release(); return 3; }
    CreateDirectoryA(outDir, nullptr);
    int ci = CreateImageInstance(&image);
    std::printf("imageCreate=%d ptr=%p\n",ci,(void*)image);
    if(ci!=0 || !image) { release(); return 4; }
    stage = "load-image";
    if (fromMemory) {
      constexpr unsigned width = 1280;
      constexpr unsigned height = 960;
      constexpr unsigned length = width * height;
      inputPixels.resize(length);
      std::ifstream buffer(file, std::ios::binary);
      if (!buffer.read(reinterpret_cast<char*>(inputPixels.data()), length) ||
          buffer.peek() != std::char_traits<char>::eof() || buffer.bad())
        throw std::runtime_error("Raw Mono8 fixture must contain exactly 1280 x 960 bytes");
      VisionDesigner::MVD_IMAGE_DATA_INFO data{};
      auto& channel = data.stDataChannel[0];
      channel.pData = inputPixels.data();
      channel.nSize = length;
      channel.nLen = length;
      channel.nRowStep = width;
      image->InitImage(width, height, VisionDesigner::MVD_PIXEL_MONO_08, data);
    } else {
      image->InitImage(file, VisionDesigner::MVD_PIXEL_MONO_08);
    }
    std::printf("inputInitialization=%s\n", fromMemory ? "memory" : "file");
    std::printf("image=%u x %u fmt=%d\n", image->GetWidth(), image->GetHeight(), (int)image->GetPixelFormat());
    stage = "capture-input";
    const auto* inputData = image->GetImageData(0);
    if (!inputData || !inputData->pData || inputData->nLen == 0)
      throw std::runtime_error("Input image has no data");
    {
      std::ofstream buffer(std::string(outDir) + "/input.bin", std::ios::binary);
      buffer.write(reinterpret_cast<const char*>(inputData->pData), inputData->nLen);
      buffer.close();
      if (!buffer) throw std::runtime_error("Cannot save input buffer");
    }
    std::printf("inputDataSize=%u inputRowStep=%u\n", inputData->nLen, inputData->nRowStep);
    tool->SetInputImage(image);
    if (generatedFullRoi) {
      stage = "generate-region-image";
      VisionDesigner::IMvdRectangleF* fullRegion = nullptr;
      const float width = static_cast<float>(image->GetWidth());
      const float height = static_cast<float>(image->GetHeight());
      const int regionCode = CreateRectangleInstance(&fullRegion, width / 2, height / 2, width, height);
      generatedRegion.reset(fullRegion);
      std::printf("generatedRegionCreate=%d\n", regionCode);
      if (regionCode != 0 || !generatedRegion) throw std::runtime_error("Cannot create full region");

      VisionDesigner::PreproMask::IPreproMaskTool* assistant = nullptr;
      const int assistantCode = CreatePreproMaskToolInstance(&assistant);
      preproMask.reset(assistant);
      std::printf("preproMaskCreate=%d\n", assistantCode);
      if (assistantCode != 0 || !preproMask) throw std::runtime_error("Cannot create mask assistant");

      preproMask->ClearRegions();
      preproMask->AddRegion(generatedRegion.get(), true);
      preproMask->SetInputImage(image);
      preproMask->Run();
      auto* regionImage = preproMask->GetOutputImage();
      if (!regionImage) throw std::runtime_error("Mask assistant returned no image");
      const auto* data = regionImage->GetImageData(0);
      if (!data || !data->pData || data->nLen == 0) throw std::runtime_error("Mask image has no data");
      std::ofstream buffer(std::string(outDir) + "/region.bin", std::ios::binary);
      if (!buffer.write(reinterpret_cast<const char*>(data->pData), data->nLen))
        throw std::runtime_error("Cannot save mask buffer");
      std::printf("region=%u x %u fmt=%d bytes=%u rowStep=%u\n", regionImage->GetWidth(),
                  regionImage->GetHeight(), static_cast<int>(regionImage->GetPixelFormat()), data->nLen, data->nRowStep);
      tool->SetRegionImage(regionImage);
    }
    if (explicitFullRoi) {
      stage = "set-full-roi";
      int rc = CreateRectangleInstance(&roi, 640.0f, 480.0f, 1280.0f, 960.0f);
      std::printf("fullRoiCreate=%d\n", rc);
      if (rc != 0 || !roi) { release(); return 6; }
      tool->SetROI(roi);
    }
    if (outsideMask) {
      stage = "set-outside-mask";
      int rc = CreatePolygonInstance(&mask);
      std::printf("outsideMaskCreate=%d\n", rc);
      if (rc != 0 || !mask) { release(); return 7; }
      mask->AddVertex(-200.0f, -200.0f);
      mask->AddVertex(-100.0f, -200.0f);
      mask->AddVertex(-100.0f, -100.0f);
      mask->AddVertex(-200.0f, -100.0f);
      tool->AddMask(mask);
    }
    if (insideMask) {
      stage = "set-inside-mask";
      int rc = CreatePolygonInstance(&mask);
      std::printf("insideMaskCreate=%d\n", rc);
      if (rc != 0 || !mask) { release(); return 8; }
      mask->AddVertex(600.0f, 300.0f);
      mask->AddVertex(680.0f, 300.0f);
      mask->AddVertex(680.0f, 460.0f);
      mask->AddVertex(600.0f, 460.0f);
      tool->AddMask(mask);
    }
    stage = "configure-blob";
    const char* pairs[][2]={{"ThresholdType","2"},{"LowThreshold","100"},{"HightThreshold","150"},{"Polarity","0"},{"connectivity","8"},{"MinArea","10"},{"MaxArea","9999999"},{"FindNum","100"}};
    for(auto& p:pairs) tool->SetRunParam(p[0],p[1]);
    if(paramFile){ std::ifstream pf(paramFile); std::string line; while(std::getline(pf,line)){ auto eq=line.find("="); if(eq==std::string::npos) continue; auto key=line.substr(0,eq); auto value=line.substr(eq+1); if(!key.empty()){ try { tool->SetRunParam(key.c_str(),value.c_str()); } catch(...) { std::printf("paramException %s=%s\n",key.c_str(),value.c_str()); } } } }
    auto* basic = tool->GetBasicParam();
    if (!basic) throw std::runtime_error("Blob basic parameters unavailable");
    basic->SetShowBlobImageStatus(imageFlags[1] == '1');
    basic->SetShowBinaryImageStatus(imageFlags[0] == '1');
    std::printf("outputFlags=%s\n", imageFlags.c_str());
    stage = "run-blob";
    tool->Run();
    stage = "capture-loaded-modules";
    if (!CaptureLoadedModules(outDir)) throw std::runtime_error("Cannot capture loaded modules");
    std::printf("loadedModulesCaptured=1\n");
    stage = "read-blob-output";
    auto* result=tool->GetResult();
    if(!result){
      std::printf("result=null\n");
      release();
      return 5;
    }
    int n=result->GetBlobNum();
    std::printf("result count=%d total=%d\n", n, result->GetBlobTotalArea());
    for(int i=0;i<n;i++){
      auto* b=result->GetBlobInfo(i); auto p=b->GetCenteroid();
      std::printf("blob[%d] area=%d areaF=%.7g peri=%.7g cx=%.7g cy=%.7g angle=%.7g long=%.7g short=%.7g circ=%.7g rect=%.7g\n",i,b->GetArea(),b->GetAreaF(),b->GetPerimeter(),p.fX,p.fY,b->GetAngle(),b->GetLongAxis(),b->GetShortAxis(),b->GetCircularity(),b->GetRectangularity());
      if (explicitFullRoi) {
        if (auto* box = b->GetBoxInfo())
          std::printf("projected-angle[%d] box=%.7g blob=%.7g\n", i, box->GetAngle(), b->GetAngle());
      }
    }
    CaptureOutputImage(outDir, "binary.bin", "binary", result->GetBinaryImage());
    CaptureOutputImage(outDir, "blob.bin", "blobImage", result->GetBlobImage());
    release();
    return 0;
  } catch(...) {
    if (std::strcmp(stage, "run-blob") == 0 && CaptureLoadedModules(outDir))
      std::printf("loadedModulesCaptured=1\n");
    std::printf("exception stage=%s global=%d create=%d tool=%p image=%p\n",stage,g,c,(void*)tool,(void*)image);
    release();
    return 2;
  }
}









