
void FUN_180121720(undefined8 param_1,int *param_2,char *param_3,undefined4 param_4,char *param_5,
                  char *param_6,char *param_7,undefined4 *param_8,undefined4 *param_9)

{
  longlong *plVar1;
  int *piVar2;
  int iVar3;
  longlong *plVar4;
  char cVar5;
  undefined4 uVar6;
  int iVar7;
  undefined8 uVar8;
  longlong lVar9;
  undefined1 *puVar10;
  longlong lVar11;
  undefined1 auStack_138 [32];
  char *pcStack_118;
  char *pcStack_110;
  char *pcStack_108;
  undefined4 *puStack_100;
  undefined4 *puStack_f8;
  undefined4 *puStack_f0;
  undefined4 uStack_e8;
  undefined4 uStack_e4;
  undefined4 *puStack_e0;
  int *piStack_d8;
  longlong *plStack_d0;
  char *pcStack_c8;
  undefined8 uStack_c0;
  char *pcStack_b8;
  undefined8 uStack_b0;
  undefined1 uStack_a8;
  undefined7 uStack_a7;
  undefined8 uStack_98;
  ulonglong uStack_90;
  undefined1 uStack_88;
  undefined7 uStack_87;
  undefined8 uStack_78;
  ulonglong uStack_70;
  undefined8 auStack_68 [3];
  ulonglong uStack_50;
  ulonglong uStack_48;
  
  uStack_b0 = 0xfffffffffffffffe;
  uStack_48 = DAT_1802a5230 ^ (ulonglong)auStack_138;
  pcStack_c8 = param_5;
  pcStack_b8 = param_7;
  puStack_e0 = param_8;
  uStack_e4 = param_4;
  if (((param_3 == (char *)0x0) || (param_6 == (char *)0x0)) || (param_7 == (char *)0x0))
  goto LAB_180121b6d;
  iVar7 = *param_2;
  uStack_c0 = *(undefined8 *)(param_2 + 2);
  if (iVar7 == -1) {
    cVar5 = FUN_18001dbc0(4);
    if (cVar5 != '\0') {
      uVar8 = FUN_18001db50();
      pcStack_118 = (char *)CONCAT44(pcStack_118._4_4_,0x4df);
      pcStack_110 = param_3;
      FUN_180015b40(uVar8,4,&DAT_180231810,&DAT_180231cb0);
    }
    goto LAB_180121b6d;
  }
  uStack_90 = 0xf;
  lVar9 = 0;
  uStack_98 = 0;
  uStack_a8 = 0;
  uVar6 = 0;
  lVar11 = lVar9;
  if (*param_3 != '\0') {
    lVar11 = -1;
    do {
      lVar11 = lVar11 + 1;
    } while (param_3[lVar11] != '\0');
  }
  FUN_180006300(&uStack_a8,param_3,lVar11);
  cVar5 = FUN_18012cd50();
  if (cVar5 != '\0') {
    uVar8 = FUN_18008e960();
    puStack_f8 = param_9;
    puStack_100 = puStack_e0;
    pcStack_108 = param_7;
    pcStack_110 = param_6;
    pcStack_118 = pcStack_c8;
    FUN_1800caa60(uVar8,iVar7,&uStack_a8,uStack_e4);
    if (0xf < uStack_90) {
      __3_YAXPEAX_Z(CONCAT71(uStack_a7,uStack_a8));
    }
    goto LAB_180121b6d;
  }
  uVar8 = FUN_18008e9a0();
  FUN_1800fb040(uVar8,&piStack_d8,iVar7,&uStack_a8);
  if (piStack_d8 == (int *)0x0) {
    cVar5 = FUN_18001dbc0(0);
    if (cVar5 != '\0') {
      pcStack_110 = (char *)FUN_18008f750(auStack_68,param_2);
      if (0xf < *(ulonglong *)(pcStack_110 + 0x18)) {
        pcStack_110 = *(char **)pcStack_110;
      }
      uVar8 = FUN_18001db50();
      pcStack_118 = (char *)CONCAT44(pcStack_118._4_4_,0x4ec);
      pcStack_108 = param_3;
      FUN_180015b40(uVar8,0,&DAT_180231860,&DAT_180231cb0);
      if (0xf < uStack_50) {
        __3_YAXPEAX_Z(auStack_68[0]);
      }
    }
    plVar4 = plStack_d0;
    if (plStack_d0 != (longlong *)0x0) {
      LOCK();
      plVar1 = plStack_d0 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plStack_d0)(plStack_d0);
        LOCK();
        piVar2 = (int *)((longlong)plVar4 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plStack_d0 + 8))();
        }
      }
    }
    if (0xf < uStack_90) {
      __3_YAXPEAX_Z(CONCAT71(uStack_a7,uStack_a8));
    }
    goto LAB_180121b6d;
  }
  uStack_e8 = 0;
  if (*piStack_d8 == 3) {
    iVar7 = piStack_d8[0x23];
    piVar2 = piStack_d8 + 0x24;
    uStack_70 = 0xf;
    uStack_78 = 0;
    uStack_88 = 0;
    if ((char)*piVar2 != '\0') {
      lVar9 = -1;
      do {
        lVar9 = lVar9 + 1;
      } while (*(char *)((longlong)piVar2 + lVar9) != '\0');
    }
    FUN_180006300(&uStack_88,piVar2,lVar9);
    uVar6 = FUN_180124d40();
    iVar3 = piStack_d8[1];
    uStack_e4 = uVar6;
    cVar5 = FUN_180125040();
    if (cVar5 == '\0') {
      uVar8 = FUN_18008e5d0();
      cVar5 = FUN_180108b20(uVar8,iVar7);
      if (cVar5 != '\0') goto LAB_180121a87;
      uVar8 = FUN_18008e6c0(uStack_c0);
      puStack_f8 = param_9;
      puStack_100 = puStack_e0;
      pcStack_108 = (char *)&uStack_e8;
      pcStack_110 = param_6;
      pcStack_118 = pcStack_c8;
      iVar7 = FUN_1800caa60(uVar8,iVar7,&uStack_88,uVar6);
    }
    else {
LAB_180121a87:
      uVar6 = FUN_18012f860();
      puVar10 = &uStack_88;
      if (0xf < uStack_70) {
        puVar10 = (undefined1 *)CONCAT71(uStack_87,uStack_88);
      }
      uVar8 = FUN_18008f040();
      puStack_f0 = param_9;
      puStack_f8 = puStack_e0;
      puStack_100 = &uStack_e8;
      pcStack_108 = param_6;
      pcStack_110 = pcStack_c8;
      pcStack_118 = (char *)CONCAT44(pcStack_118._4_4_,uStack_e4);
      iVar7 = FUN_1800ef410(uVar8,iVar7,puVar10,uVar6);
    }
    if ((iVar7 == 0) && (iVar3 != 1)) {
      uStack_e8 = 1;
    }
    uVar6 = uStack_e8;
    if (0xf < uStack_70) {
      __3_YAXPEAX_Z(CONCAT71(uStack_87,uStack_88));
    }
  }
  else {
    cVar5 = FUN_18001dbc0(4);
    if (cVar5 != '\0') {
      uVar8 = FUN_18001db50();
      pcStack_118 = (char *)CONCAT44(pcStack_118._4_4_,0x515);
      pcStack_110 = param_3;
      FUN_180015b40(uVar8,4,&DAT_180224860,&DAT_180231cb0);
    }
    uStack_e8 = 0;
  }
  plVar4 = plStack_d0;
  *(undefined4 *)pcStack_b8 = uVar6;
  if (plStack_d0 != (longlong *)0x0) {
    LOCK();
    plVar1 = plStack_d0 + 1;
    *(int *)plVar1 = (int)*plVar1 + -1;
    UNLOCK();
    if ((int)*plVar1 == 0) {
      (**(code **)*plStack_d0)(plStack_d0);
      LOCK();
      piVar2 = (int *)((longlong)plVar4 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 == 0) {
        (**(code **)(*plStack_d0 + 8))();
      }
    }
  }
  if (0xf < uStack_90) {
    __3_YAXPEAX_Z(CONCAT71(uStack_a7,uStack_a8));
  }
LAB_180121b6d:
  FUN_1801e8810(uStack_48 ^ (ulonglong)auStack_138);
  return;
}

