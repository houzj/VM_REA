#include <cstdio>
#include <cstring>
#include <fstream>
#include <string>
#include <windows.h>
#include "Algorithms/BlobFindCpp.h"
#include "Algorithms/GlobalInitCpp.h"
#include "MVDImageCpp.h"
#include "MVDShapeCpp.h"
#include "MVD_ImageParamDefine.h"

int main(int argc, char** argv) {
  const char* file = argc > 1 ? argv[1] : "D:\\MyWork\\图片库\\标定\\1.jpg";
  const char* outDir = argc > 2 ? argv[2] : "analysis/full-audit/batch1-20261107/C03/native-sdk-run";
  const char* paramFile = argc > 3 ? argv[3] : nullptr;
  const bool explicitFullRoi = argc > 4 && std::strcmp(argv[4], "full-roi") == 0;
  const bool outsideMask = argc > 4 && std::strcmp(argv[4], "outside-mask") == 0;
  void* global = nullptr;
  VisionDesigner::BlobFind::IBlobFindTool* tool = nullptr;
  VisionDesigner::IMvdImage* image = nullptr;
  int g=-1,c=-1;
  try {
    g = CreateGlobalInitToolInstance(&global);
    c = CreateBlobFindToolInstance(&tool);
    std::printf("{\"global\":%d,\"create\":%d,\"file\":\"%s\"}\n",g,c,file);
    if(c!=0 || !tool) return 3;
    CreateDirectoryA(outDir, nullptr);
    int ci = CreateImageInstance(&image);
    std::printf("imageCreate=%d ptr=%p\n",ci,(void*)image);
    if(ci!=0 || !image) return 4;
    image->InitImage(file, VisionDesigner::MVD_PIXEL_MONO_08);
    std::printf("image=%u x %u fmt=%d\n", image->GetWidth(), image->GetHeight(), (int)image->GetPixelFormat());
    tool->SetInputImage(image);
    VisionDesigner::IMvdRectangleF* roi = nullptr;
    if (explicitFullRoi) {
      int rc = CreateRectangleInstance(&roi, 640.0f, 480.0f, 1280.0f, 960.0f);
      std::printf("fullRoiCreate=%d\n", rc);
      if (rc != 0 || !roi) return 6;
      tool->SetROI(roi);
    }
    VisionDesigner::IMvdPolygonF* mask = nullptr;
    if (outsideMask) {
      int rc = CreatePolygonInstance(&mask);
      std::printf("outsideMaskCreate=%d\n", rc);
      if (rc != 0 || !mask) return 7;
      mask->AddVertex(-200.0f, -200.0f);
      mask->AddVertex(-100.0f, -200.0f);
      mask->AddVertex(-100.0f, -100.0f);
      mask->AddVertex(-200.0f, -100.0f);
      tool->AddMask(mask);
    }
    const char* pairs[][2]={{"ThresholdType","2"},{"LowThreshold","100"},{"HightThreshold","150"},{"Polarity","0"},{"connectivity","8"},{"MinArea","10"},{"MaxArea","9999999"},{"FindNum","100"}};
    for(auto& p:pairs) tool->SetRunParam(p[0],p[1]);
    if(paramFile){ std::ifstream pf(paramFile); std::string line; while(std::getline(pf,line)){ auto eq=line.find("="); if(eq==std::string::npos) continue; auto key=line.substr(0,eq); auto value=line.substr(eq+1); if(!key.empty()){ try { tool->SetRunParam(key.c_str(),value.c_str()); } catch(...) { std::printf("paramException %s=%s\n",key.c_str(),value.c_str()); } } } }
    auto* basic=tool->GetBasicParam(); if(basic){basic->SetShowBlobImageStatus(true); basic->SetShowBinaryImageStatus(true);}
    tool->Run();
    auto* result=tool->GetResult();
    if(!result){std::printf("result=null\n"); return 5;}
    int n=result->GetBlobNum();
    std::printf("result count=%d total=%d\n", n, result->GetBlobTotalArea());
    for(int i=0;i<n;i++){
      auto* b=result->GetBlobInfo(i); auto p=b->GetCenteroid();
      std::printf("blob[%d] area=%d areaF=%.7g peri=%.7g cx=%.7g cy=%.7g angle=%.7g long=%.7g short=%.7g circ=%.7g rect=%.7g\n",i,b->GetArea(),b->GetAreaF(),b->GetPerimeter(),p.fX,p.fY,b->GetAngle(),b->GetLongAxis(),b->GetShortAxis(),b->GetCircularity(),b->GetRectangularity());
    }
    auto* bi=result->GetBinaryImage(); auto* bl=result->GetBlobImage();
    if(bi){ std::printf("binary=%u x %u fmt=%d\n",bi->GetWidth(),bi->GetHeight(),(int)bi->GetPixelFormat()); auto* d=bi->GetImageData(); std::ofstream f(std::string(outDir)+"/binary.bin",std::ios::binary); f.write((const char*)d->stDataChannel[0].pData,d->stDataChannel[0].nLen); std::printf("binaryDataSize=%u binaryRowStep=%u\n",d->stDataChannel[0].nLen,d->stDataChannel[0].nRowStep); } else std::printf("binary=null\n");
    if(bl){ std::printf("blobImage=%u x %u fmt=%d\n",bl->GetWidth(),bl->GetHeight(),(int)bl->GetPixelFormat()); auto* d=bl->GetImageData(); std::ofstream f(std::string(outDir)+"/blob.bin",std::ios::binary); f.write((const char*)d->stDataChannel[0].pData,d->stDataChannel[0].nLen); std::printf("blobDataSize=%u blobRowStep=%u\n",d->stDataChannel[0].nLen,d->stDataChannel[0].nRowStep); } else std::printf("blobImage=null\n");
    DestroyImageInstance(image); image=nullptr;
    if (roi) DestroyRectangleInstance(roi);
    if (mask) DestroyPolygonInstance(mask);
    DestroyBlobFindToolInstance(tool); tool=nullptr;
    if(global) DestroyGlobalInitToolInstance(global);
    return 0;
  } catch(...) {
    std::printf("exception global=%d create=%d tool=%p image=%p\n",g,c,(void*)tool,(void*)image);
    if(image) DestroyImageInstance(image); if(tool) DestroyBlobFindToolInstance(tool); if(global) DestroyGlobalInitToolInstance(global);
    return 2;
  }
}









