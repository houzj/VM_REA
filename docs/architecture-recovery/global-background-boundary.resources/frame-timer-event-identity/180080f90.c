
void FUN_180080f90(longlong param_1)

{
  char cVar1;
  int iVar2;
  undefined8 uVar3;
  
  iVar2 = _Mtx_lock();
  if (iVar2 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar2);
  }
  if (*(longlong *)(param_1 + 0x28) != *(longlong *)(param_1 + 0x30)) {
    cVar1 = FUN_18001dbc0(0);
    if (cVar1 != '\0') {
      uVar3 = FUN_18001db50();
      FUN_180015b40(uVar3,0,&UNK_180224f48,&UNK_180224f30,0x129);
    }
    iVar2 = SetEvent(*(undefined8 *)(param_1 + 0x100));
    if (iVar2 == 0) {
      GetLastError();
    }
  }
  iVar2 = _Mtx_unlock(param_1);
  if (iVar2 != 0) {
                    /* WARNING: Could not recover jumptable at 0x000180081020. Too many branches */
                    /* WARNING: Treating indirect jump as call */
    __Throw_C_error_std__YAXH_Z(iVar2);
    return;
  }
  return;
}

