
/* WARNING: Function: __security_check_cookie replaced with injection: security_check_cookie */
/* public: int __cdecl CImageBufferModule::SetOutImage(void * __ptr64,struct ImageData,class
   std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> >) __ptr64 */

int __thiscall
CImageBufferModule::SetOutImage
          (undefined8 param_1_00,undefined8 param_1,undefined4 *param_3,undefined8 *param_4)

{
  int iVar1;
  ulonglong *puVar2;
  undefined1 auStack_118 [32];
  undefined8 local_f8;
  undefined8 local_e8;
  undefined8 *local_e0;
  undefined8 local_d8;
  undefined4 local_d0 [34];
  void *local_48 [3];
  ulonglong local_30;
  ulonglong local_28;
  
                    /* 0xe760  20
                       ?SetOutImage@CImageBufferModule@@QEAAHPEAXUImageData@@V?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@@Z
                        */
  local_e8 = 0xfffffffffffffffe;
  local_28 = DAT_180019000 ^ (ulonglong)auStack_118;
  local_e0 = param_4;
  puVar2 = FUN_180002ed0((ulonglong *)local_48,param_4,"Width");
  if (0xf < puVar2[3]) {
    puVar2 = (ulonglong *)*puVar2;
  }
  iVar1 = VM_M_SetInt(param_1,puVar2,0,*param_3);
  if (0xf < local_30) {
    operator_delete(local_48[0]);
  }
  if (iVar1 == 0) {
    puVar2 = FUN_180002ed0((ulonglong *)local_48,param_4,"Height");
    if (0xf < puVar2[3]) {
      puVar2 = (ulonglong *)*puVar2;
    }
    iVar1 = VM_M_SetInt(param_1,puVar2,0,param_3[1]);
    if (0xf < local_30) {
      operator_delete(local_48[0]);
    }
    if (iVar1 == 0) {
      puVar2 = FUN_180002ed0((ulonglong *)local_48,param_4,"PixelFormat");
      if (0xf < puVar2[3]) {
        puVar2 = (ulonglong *)*puVar2;
      }
      iVar1 = VM_M_SetInt(param_1,puVar2,0,param_3[2]);
      if (0xf < local_30) {
        operator_delete(local_48[0]);
      }
      if (iVar1 == 0) {
        memset(local_d0,0,0x88);
        local_d8 = *(undefined8 *)(param_3 + 4);
        local_d0[0] = param_3[3];
        puVar2 = FUN_180002ed0((ulonglong *)local_48,param_4,"Image");
        if (0xf < puVar2[3]) {
          puVar2 = (ulonglong *)*puVar2;
        }
        local_f8 = *(undefined8 *)(param_3 + 8);
        iVar1 = VM_M_SetImageEx(param_1,puVar2,0,&local_d8);
        if (0xf < local_30) {
          operator_delete(local_48[0]);
        }
        FUN_180001ee0(param_4);
        if (iVar1 != 0) {
          return iVar1;
        }
        return 0;
      }
    }
  }
  if (0xf < (ulonglong)param_4[3]) {
    operator_delete((void *)*param_4);
  }
  param_4[3] = 0xf;
  param_4[2] = 0;
  *(undefined1 *)param_4 = 0;
  return iVar1;
}

