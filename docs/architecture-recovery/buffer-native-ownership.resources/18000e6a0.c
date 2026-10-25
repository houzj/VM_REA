
/* public: int __cdecl CImageBufferModule::CopyImage(struct ImageData & __ptr64,struct ImageData
   const & __ptr64) __ptr64 */

int __thiscall
CImageBufferModule::CopyImage(CImageBufferModule *this,ImageData *param_1,ImageData *param_2)

{
  int iVar1;
  undefined8 local_res8;
  
                    /* 0xe6a0  7  ?CopyImage@CImageBufferModule@@QEAAHAEAUImageData@@AEBU2@@Z */
  local_res8 = 0;
  iVar1 = (**(code **)(*(longlong *)(this + 0x50) + 8))
                    (this + 0x50,*(undefined4 *)(this + 0x48),(longlong)*(int *)(param_2 + 0xc),
                     param_1 + 0x10,&local_res8,0);
  *(undefined4 *)(param_1 + 0x18) = *(undefined4 *)(param_2 + 0xc);
  if (iVar1 != 0) {
    *(undefined4 *)(param_1 + 0x18) = 0;
    return -0x1ffffffe;
  }
  memcpy_s(*(void **)(param_1 + 0x10),(longlong)*(int *)(param_1 + 0x18),*(void **)(param_2 + 0x10),
           (longlong)*(int *)(param_2 + 0xc));
  *(undefined4 *)(param_1 + 0xc) = *(undefined4 *)(param_2 + 0xc);
  *(undefined4 *)param_1 = *(undefined4 *)param_2;
  *(undefined4 *)(param_1 + 4) = *(undefined4 *)(param_2 + 4);
  *(undefined4 *)(param_1 + 8) = *(undefined4 *)(param_2 + 8);
  *(undefined8 *)(param_1 + 0x20) = local_res8;
  return 0;
}

