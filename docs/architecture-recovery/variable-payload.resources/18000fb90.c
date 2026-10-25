
void FUN_18000fb90(longlong *param_1)

{
  longlong *plVar1;
  longlong lVar2;
  
  if ((char)param_1[0xf] != '\0') {
    if ((longlong *)param_1[6] == (longlong *)0x0) {
                    /* WARNING: Subroutine does not return */
      __Xbad_function_call_std__YAXXZ();
    }
    lVar2 = (**(code **)(*(longlong *)param_1[6] + 0x10))();
    param_1[2] = lVar2;
    *(uint *)(lVar2 + 0x10) = *(uint *)(lVar2 + 0x10) | 1;
    plVar1 = *(longlong **)(lVar2 + 0x18);
    if (plVar1 == DAT_1802b05e8) {
      FUN_180001ac0(lVar2 + 0x18);
    }
    else if (plVar1 != param_1 + 7) {
      FUN_180006470(plVar1,param_1 + 7,0,0xffffffffffffffff);
    }
    lVar2 = param_1[2];
    *(uint *)(lVar2 + 0x10) = *(uint *)(lVar2 + 0x10) | 2;
    plVar1 = *(longlong **)(lVar2 + 0x20);
    if (plVar1 == DAT_1802b05e8) {
      FUN_180001ac0(lVar2 + 0x20);
      (**(code **)(*param_1 + 0x10))(param_1);
      *(undefined1 *)(param_1 + 0xf) = 0;
      return;
    }
    if (plVar1 != param_1 + 0xb) {
      FUN_180006470(plVar1,param_1 + 0xb,0,0xffffffffffffffff);
    }
    (**(code **)(*param_1 + 0x10))(param_1);
    *(undefined1 *)(param_1 + 0xf) = 0;
  }
  return;
}

