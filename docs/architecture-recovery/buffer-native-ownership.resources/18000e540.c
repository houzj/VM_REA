
/* WARNING: Function: __security_check_cookie replaced with injection: security_check_cookie */
/* public: int __cdecl CImageBufferModule::GetInImage(void * __ptr64,struct ImageData & __ptr64)
   __ptr64 */

int __thiscall
CImageBufferModule::GetInImage(CImageBufferModule *this,void *param_1,ImageData *param_2)

{
  int iVar1;
  undefined1 auStack_e8 [32];
  undefined4 *local_c8;
  undefined4 local_b8;
  undefined4 local_b4;
  undefined4 local_b0;
  undefined4 local_ac;
  longlong local_a8;
  int local_a0 [34];
  ulonglong local_18;
  
                    /* 0xe540  10  ?GetInImage@CImageBufferModule@@QEAAHPEAXAEAUImageData@@@Z */
  local_18 = DAT_180019000 ^ (ulonglong)auStack_e8;
  local_c8 = &local_b8;
  local_b4 = 0;
  local_ac = 0;
  local_b0 = 0;
  local_b8 = 0;
  iVar1 = VM_M_GetInt(param_1,"InImageWidth",0,&local_b4);
  if (iVar1 == 0) {
    local_c8 = &local_b8;
    iVar1 = VM_M_GetInt(param_1,"InImageHeight",0,&local_ac);
    if (iVar1 == 0) {
      local_c8 = &local_b8;
      iVar1 = VM_M_GetInt(param_1,"InImagePixelFormat",0,&local_b0);
      if (iVar1 == 0) {
        local_a8 = 0;
        memset(local_a0,0,0x88);
        local_c8 = &local_b8;
        iVar1 = VM_M_GetImage(param_1,"InImage",0,&local_a8);
        if (((iVar1 == 0) && (local_a8 != 0)) && (local_a0[0] != 0)) {
          *(int *)(param_2 + 0xc) = local_a0[0];
          *(int *)(param_2 + 0x18) = local_a0[0];
          *(undefined4 *)(param_2 + 4) = local_ac;
          *(longlong *)(param_2 + 0x10) = local_a8;
          *(undefined4 *)param_2 = local_b4;
          *(undefined4 *)(param_2 + 8) = local_b0;
          return 0;
        }
      }
    }
  }
  return -0x1ffffaf0;
}

