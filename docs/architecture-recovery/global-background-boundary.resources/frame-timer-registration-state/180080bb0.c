
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180080bb0(longlong param_1,undefined4 param_2,longlong *param_3)

{
  longlong *plVar1;
  bool bVar2;
  char cVar3;
  int iVar4;
  undefined8 uVar5;
  longlong lVar6;
  undefined8 uVar7;
  longlong *plVar8;
  double dVar9;
  undefined1 auStack_c8 [32];
  undefined4 uStack_a8;
  undefined4 uStack_a0;
  undefined8 uStack_98;
  longlong *plStack_88;
  longlong lStack_80;
  undefined8 uStack_78;
  longlong lStack_70;
  undefined8 uStack_68;
  longlong alStack_60 [3];
  longlong *plStack_48;
  ulonglong uStack_40;
  
  uStack_78 = 0xfffffffffffffffe;
  uStack_40 = _DAT_1802a5230 ^ (ulonglong)auStack_c8;
  plStack_88 = param_3;
  if (*(char *)(param_1 + 0x110) == '\0') {
    FUN_18007ff30();
  }
  bVar2 = true;
  uVar5 = FUN_1800810d0(param_1);
  lVar6 = FUN_180081030(param_1,param_2);
  cVar3 = FUN_18001dbc0(0);
  if (cVar3 != '\0') {
    uVar7 = FUN_18001db50();
    uStack_a8 = 0xf7;
    uStack_a0 = param_2;
    uStack_98 = uVar5;
    FUN_180015b40(uVar7,0,&UNK_180224f00,&UNK_180224ee8);
  }
  lStack_80 = param_1;
  iVar4 = _Mtx_lock(param_1);
  if (iVar4 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar4);
  }
  if (*(longlong *)(param_1 + 0x28) != *(longlong *)(param_1 + 0x30)) {
    dVar9 = (double)*(longlong *)(param_1 + 0xf0) * _DAT_18023a728;
    bVar2 = true;
    if ((double)CONCAT44((uint)((ulonglong)dVar9 >> 0x20) ^ _UNK_18023a834,
                         SUB84(dVar9,0) ^ _DAT_18023a830) <
        (double)(lVar6 - **(longlong **)(param_1 + 0x28))) {
      bVar2 = false;
    }
  }
  plStack_48 = (longlong *)0x0;
  plVar1 = (longlong *)param_3[3];
  lStack_70 = lVar6;
  uStack_68 = uVar5;
  if (plVar1 == (longlong *)0x0) {
    plStack_48 = (longlong *)0x0;
  }
  else {
    if (plVar1 == param_3) {
      plVar8 = alStack_60;
    }
    else {
      plVar8 = (longlong *)0x0;
    }
    plStack_48 = (longlong *)(**(code **)*plVar1)(plVar1,plVar8);
  }
  FUN_180081770(param_1 + 0x28,&lStack_70);
  if (*(longlong *)(param_1 + 0x28) != *(longlong *)(param_1 + 0x30)) {
    FUN_180082400(*(longlong *)(param_1 + 0x28),*(longlong *)(param_1 + 0x30) + -0x30,
                  *(undefined1 *)(param_1 + 0x40));
  }
  if (plStack_48 != (longlong *)0x0) {
    (**(code **)(*plStack_48 + 0x20))
              (plStack_48,CONCAT71((int7)((ulonglong)alStack_60 >> 8),plStack_48 != alStack_60));
    plStack_48 = (longlong *)0x0;
  }
  iVar4 = _Mtx_unlock(param_1);
  if (iVar4 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar4);
  }
  if (bVar2) {
    FUN_180080f90(param_1);
  }
  plVar1 = (longlong *)param_3[3];
  if (plVar1 != (longlong *)0x0) {
    (**(code **)(*plVar1 + 0x20))(plVar1,plVar1 != param_3);
    param_3[3] = 0;
  }
  plVar1 = (longlong *)param_3[3];
  if (plVar1 != (longlong *)0x0) {
    (**(code **)(*plVar1 + 0x20))(plVar1,plVar1 != param_3);
    param_3[3] = 0;
  }
  FUN_1801e8810(uStack_40 ^ (ulonglong)auStack_c8);
  return;
}

