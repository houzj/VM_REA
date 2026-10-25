
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800de180(longlong param_1,int param_2,float param_3,undefined4 param_4)

{
  longlong *plVar1;
  int *piVar2;
  int iVar3;
  double dVar4;
  longlong lVar5;
  char cVar6;
  undefined8 uVar7;
  undefined8 *puVar8;
  double *pdVar9;
  longlong lVar10;
  bool bVar11;
  undefined4 extraout_XMM0_Da;
  undefined1 auStack_5c8 [32];
  double *pdStack_5a8;
  double *pdStack_5a0;
  undefined8 *puStack_598;
  double dStack_590;
  undefined4 uStack_588;
  undefined4 uStack_580;
  undefined8 uStack_578;
  undefined4 uStack_570;
  longlong lStack_568;
  undefined8 uStack_558;
  undefined1 auStack_550 [8];
  longlong *plStack_548;
  undefined1 uStack_540;
  undefined7 uStack_53f;
  undefined8 uStack_530;
  ulonglong uStack_528;
  undefined8 auStack_520 [3];
  ulonglong uStack_508;
  undefined4 uStack_4f8;
  int iStack_4f4;
  float fStack_4f0;
  undefined4 uStack_4ec;
  undefined4 uStack_4e8;
  undefined4 uStack_4e4;
  undefined1 auStack_4e0 [120];
  char acStack_468 [1024];
  ulonglong uStack_68;
  
  uStack_558 = 0xfffffffffffffffe;
  uStack_68 = _DAT_1802a5230 ^ (ulonglong)auStack_5c8;
  func_0x0001801e9194(&iStack_4f4,0,0x8c);
  uStack_4f8 = *(undefined4 *)(param_1 + 0x50);
  uStack_4ec = *(undefined4 *)(param_1 + 0xe0);
  iStack_4f4 = param_2;
  fStack_4f0 = param_3;
  uStack_4e8 = param_4;
  if ((_DAT_1802b1888 & 1) == 0) {
    _DAT_1802b1888 = _DAT_1802b1888 | 1;
    FUN_180107fc0();
    FUN_1801e8930(&UNK_180202980);
  }
  uStack_4e4 = _DAT_1802b19e0;
  if (*(int *)(param_1 + 0x50) != 100000) {
    uStack_4e4 = ((_IMAGE_RUNTIME_FUNCTION_ENTRY *)
                 ((longlong)&_IMAGE_RUNTIME_FUNCTION_ENTRY_1802bc5bc +
                 (longlong)*(int *)(param_1 + 0x50) * -4))->BeginAddress;
  }
  cVar6 = FUN_18001dbc0(2);
  if (cVar6 != '\0') {
    puStack_598 = (undefined8 *)FUN_1800de580(extraout_XMM0_Da,auStack_520,param_2);
    lStack_568 = param_1 + 0xa8;
    if (0xf < *(ulonglong *)(param_1 + 0xc0)) {
      lStack_568 = *(longlong *)lStack_568;
    }
    if (0xf < (ulonglong)puStack_598[3]) {
      puStack_598 = (undefined8 *)*puStack_598;
    }
    uVar7 = FUN_18001db50();
    uStack_570 = uStack_4e4;
    uStack_578 = *(undefined8 *)(param_1 + 0x198);
    uStack_580 = *(undefined4 *)(param_1 + 0xe0);
    pdStack_5a0 = (double *)CONCAT44(pdStack_5a0._4_4_,*(undefined4 *)(param_1 + 0x50));
    pdStack_5a8 = (double *)CONCAT44(pdStack_5a8._4_4_,0x24d);
    dStack_590 = (double)param_3;
    uStack_588 = param_4;
    FUN_180015b40(uVar7,2,&UNK_18022cc50,&UNK_18022ccd8);
    if (0xf < uStack_508) {
      __3_YAXPEAX_Z(auStack_520[0]);
    }
  }
  *(int *)(param_1 + 0x110) = param_2;
  pdVar9 = (double *)(param_1 + 0xa8);
  pdStack_5a8 = pdVar9;
  if (0xf < *(ulonglong *)(param_1 + 0xc0)) {
    pdStack_5a8 = (double *)*pdVar9;
  }
  _snprintf_s(auStack_4e0,0x10,0xffffffffffffffff,&UNK_18021dae0);
  if (param_2 == 2) {
    acStack_468[0] = '\0';
    func_0x0001801e9194(acStack_468 + 1,0,0x3ff);
    if (0xf < *(ulonglong *)(param_1 + 0xc0)) {
      pdVar9 = (double *)*pdVar9;
    }
    pdStack_5a8 = (double *)CONCAT44(pdStack_5a8._4_4_,*(undefined4 *)(param_1 + 0xe0));
    pdStack_5a0 = pdVar9;
    sprintf_s(acStack_468,0x3ff,&UNK_18022cd28,*(undefined4 *)(param_1 + 0x50));
    uStack_528 = 0xf;
    uStack_530 = 0;
    uStack_540 = 0;
    if (acStack_468[0] != '\0') {
      lVar10 = -1;
      do {
        lVar5 = lVar10 + 1;
        lVar10 = lVar10 + 1;
      } while (acStack_468[lVar5] != '\0');
    }
    FUN_180006300(&uStack_540,acStack_468);
    puVar8 = (undefined8 *)FUN_1800d33b0(auStack_550);
    FUN_1800d3780(*puVar8,*(undefined4 *)(param_1 + 0x50),*(undefined8 *)(param_1 + 0x198),
                  &uStack_540);
    if (plStack_548 != (longlong *)0x0) {
      LOCK();
      plVar1 = plStack_548 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plStack_548)(plStack_548);
        LOCK();
        piVar2 = (int *)((longlong)plStack_548 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plStack_548 + 8))(plStack_548);
        }
      }
    }
    if (0xf < uStack_528) {
      __3_YAXPEAX_Z(CONCAT71(uStack_53f,uStack_540));
    }
  }
  else {
    if (param_2 == 1) {
      dVar4 = (double)FUN_18008f820();
      pdVar9 = (double *)((dVar4 - *(double *)(param_1 + 0x178)) - (double)param_3);
      if ((_DAT_18023a790 < (double)pdVar9) && (cVar6 = FUN_18001dbc0(3), cVar6 != '\0')) {
        uVar7 = FUN_18001db50();
        pdStack_5a8 = (double *)CONCAT44(pdStack_5a8._4_4_,0x260);
        pdStack_5a0 = pdVar9;
        FUN_180015b40(uVar7,3,&UNK_18022cd00,&UNK_18022ccd8);
      }
    }
    if (_DAT_1802a5258 != (code *)0x0) {
      (*_DAT_1802a5258)(6,&uStack_4f8,_DAT_1802b1180);
    }
    if (param_2 == 3) {
      if ((_DAT_1802b12c0 & 1) == 0) {
        _DAT_1802b12c0 = _DAT_1802b12c0 | 1;
        FUN_18003fde0();
        FUN_1801e8930(&UNK_180202970);
      }
      LOCK();
      iVar3 = _DAT_1802b16c8 + -1;
      UNLOCK();
      bVar11 = _DAT_1802b16c8 == 1;
      _DAT_1802b16c8 = iVar3;
      if (bVar11) {
        cVar6 = FUN_18001dbc0(2);
        if (cVar6 != '\0') {
          uVar7 = FUN_18001db50();
          pdStack_5a8 = (double *)CONCAT44(pdStack_5a8._4_4_,0x2050);
          FUN_180015b40(uVar7,2,&UNK_1802245b0,&UNK_180224580);
        }
        if (_DAT_1802a5258 != (code *)0x0) {
          (*_DAT_1802a5258)(0x12,&UNK_18021cf35,_DAT_1802b1180);
        }
      }
    }
  }
  FUN_18003bcc0(2,&uStack_4f8);
  FUN_1801e8810(uStack_68 ^ (ulonglong)auStack_5c8);
  return;
}

