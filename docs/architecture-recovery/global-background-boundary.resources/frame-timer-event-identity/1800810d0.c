
longlong FUN_1800810d0(longlong param_1)

{
  char cVar1;
  int iVar2;
  longlong lVar3;
  undefined8 uVar4;
  longlong *plVar5;
  undefined1 auStack_40 [24];
  undefined1 auStack_28 [32];
  
  iVar2 = _Mtx_lock(param_1 + 8);
  if (iVar2 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar2);
  }
  if (*(longlong *)(param_1 + 0x68) == 0) {
    lVar3 = CreateEventW(0,0,0,0);
    if (lVar3 == 0) {
      cVar1 = FUN_18001dbc0(4);
      if (cVar1 != '\0') {
        uVar4 = FUN_18001db50();
        FUN_180015b40(uVar4,4,&UNK_180224ff0,&UNK_180224fd0,0x159);
      }
    }
  }
  else {
    uVar4 = func_0x000180081cf0(param_1 + 0x48,auStack_40);
    plVar5 = (longlong *)func_0x000180081ed0(uVar4,auStack_28);
    lVar3 = 0;
    if ((longlong *)*plVar5 != (longlong *)0x0) {
      lVar3 = *(longlong *)*plVar5;
    }
    lVar3 = *(longlong *)
             (*(longlong *)
               (*(longlong *)(lVar3 + 8) +
               (*(longlong *)(lVar3 + 0x10) - 1U & (ulonglong)plVar5[2] >> 1) * 8) +
             (ulonglong)((uint)plVar5[2] & 1) * 8);
    plVar5 = (longlong *)(param_1 + 0x68);
    *plVar5 = *plVar5 + -1;
    if (*plVar5 == 0) {
      *(undefined8 *)(param_1 + 0x60) = 0;
    }
    iVar2 = ResetEvent(lVar3);
    if (iVar2 == 0) {
      GetLastError();
    }
  }
  iVar2 = _Mtx_unlock(param_1 + 8);
  if (iVar2 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar2);
  }
  return lVar3;
}

