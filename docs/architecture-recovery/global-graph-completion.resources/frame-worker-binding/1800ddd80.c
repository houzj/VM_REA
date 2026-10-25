
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

ulonglong FUN_1800ddd80(longlong param_1,undefined4 param_2,undefined1 param_3,char param_4)

{
  int *piVar1;
  undefined8 uVar2;
  code *pcVar3;
  longlong *plVar4;
  char cVar5;
  uint uVar6;
  undefined8 *puVar7;
  longlong *plVar8;
  ulonglong uVar9;
  longlong lVar10;
  undefined8 uVar11;
  undefined1 *puVar12;
  longlong lStack_78;
  longlong *plStack_70;
  undefined8 uStack_68;
  undefined8 uStack_60;
  undefined4 uStack_58;
  undefined4 uStack_54;
  undefined8 uStack_50;
  longlong lStack_48;
  undefined1 auStack_40 [24];
  undefined8 *puStack_28;
  
  uVar11 = 0xfffffffffffffffe;
  if ((_DAT_1802b1888 & 1) == 0) {
    _DAT_1802b1888 = _DAT_1802b1888 | 1;
    FUN_180107fc0();
    FUN_1801e8930(&UNK_180202980);
  }
  FUN_18010a690(&DAT_1802b1890,&lStack_78,param_2);
  lVar10 = lStack_78;
  if (lStack_78 == 0) {
    uVar9 = 0xe0000302;
    plVar8 = plStack_70;
  }
  else {
    uVar2 = *(undefined8 *)(lStack_78 + 0xf8);
    if ((param_4 == '\0') ||
       ((cVar5 = FUN_1800dd200(param_1), cVar5 == '\0' && (*(char *)(lVar10 + 0x130) == '\0')))) {
      *(undefined1 *)(param_1 + 0x1a8) = 1;
      puVar12 = auStack_40;
      puStack_28 = (undefined8 *)0x0;
      uStack_58 = param_2;
      uStack_50 = uVar2;
      lStack_48 = param_1;
      puVar7 = (undefined8 *)__2_YAPEAX_K_Z(0x28);
      plVar8 = plStack_70;
      if (puVar7 == (undefined8 *)0x0) {
        __Xbad_alloc_std__YAXXZ();
        pcVar3 = (code *)swi(3);
        uVar9 = (*pcVar3)();
        return uVar9;
      }
      *puVar7 = &UNK_18022d5e8;
      puVar7[1] = CONCAT44(uStack_54,uStack_58);
      puVar7[2] = uStack_50;
      puVar7[3] = lStack_48;
      uStack_68 = 0;
      uStack_60 = 0;
      if (plStack_70 != (longlong *)0x0) {
        LOCK();
        *(int *)(plStack_70 + 1) = (int)plStack_70[1] + 1;
        UNLOCK();
        lVar10 = lStack_78;
      }
      puStack_28 = puVar7;
      FUN_180015c40(&uStack_68,lVar10,plStack_70);
      uVar6 = FUN_180100ad0(param_1 + 200,&uStack_68,param_3,auStack_40,uVar11,puVar12);
      uVar9 = (ulonglong)uVar6;
    }
    else {
      uVar9 = 0xe0000300;
      plVar8 = plStack_70;
    }
  }
  plVar4 = plStack_70;
  if (plVar8 != (longlong *)0x0) {
    LOCK();
    plVar8 = plVar8 + 1;
    *(int *)plVar8 = (int)*plVar8 + -1;
    UNLOCK();
    if ((int)*plVar8 == 0) {
      (**(code **)*plStack_70)(plStack_70);
      LOCK();
      piVar1 = (int *)((longlong)plVar4 + 0xc);
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (*piVar1 == 0) {
        (**(code **)(*plStack_70 + 8))();
      }
    }
  }
  return uVar9;
}

