
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_18007ff30(longlong param_1,undefined8 param_2,undefined8 param_3,undefined8 param_4)

{
  code *pcVar1;
  int iVar2;
  longlong *plVar3;
  undefined8 uVar4;
  longlong alStack_50 [3];
  longlong *plStack_38;
  undefined1 auStack_30 [24];
  undefined8 uStack_18;
  
  uVar4 = 0xfffffffffffffffe;
  if (*(char *)(param_1 + 0x110) == '\0') {
    iVar2 = _Mtx_lock(param_1 + 0x20);
    if (iVar2 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar2);
    }
    if (*(char *)(param_1 + 0x110) == '\0') {
      *(undefined1 *)(param_1 + 0x110) = 1;
      plStack_38 = (longlong *)0x0;
      plVar3 = (longlong *)__2_YAPEAX_K_Z(0x20);
      if (plVar3 == (longlong *)0x0) {
        __Xbad_alloc_std__YAXXZ();
        pcVar1 = (code *)swi(3);
        (*pcVar1)();
        return;
      }
      *plVar3 = (longlong)&UNK_180225070;
      plVar3[1] = (longlong)FUN_180080080;
      plVar3[2] = param_1;
      plStack_38 = plVar3;
      if ((_DAT_1802b2980 & 1) == 0) {
        _DAT_1802b2980 = _DAT_1802b2980 | 1;
        FUN_18015a9d0();
        FUN_1801e8930(FUN_180202990);
      }
      uStack_18 = 0;
      FUN_1800f3be0(auStack_30,alStack_50,param_3,param_4,uVar4);
      FUN_1800fb9d0(0x1802b2998,auStack_30);
      if (plStack_38 != (longlong *)0x0) {
        (**(code **)(*plStack_38 + 0x20))
                  (plStack_38,CONCAT71((int7)((ulonglong)alStack_50 >> 8),plStack_38 != alStack_50))
        ;
      }
    }
    iVar2 = _Mtx_unlock(param_1 + 0x20);
    if (iVar2 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar2);
    }
  }
  return;
}

