
void FUN_1800a55b0(longlong param_1)

{
  float fVar1;
  float fVar2;
  int iVar3;
  char cVar4;
  int iVar5;
  uint uVar6;
  undefined8 uVar7;
  undefined1 *puVar8;
  undefined1 *puVar9;
  ulonglong uVar10;
  bool bVar11;
  ulonglong uVar12;
  ulonglong uVar13;
  bool bVar14;
  undefined8 extraout_XMM0_Qa;
  undefined8 extraout_XMM0_Qa_00;
  undefined8 extraout_XMM0_Qa_01;
  undefined8 extraout_XMM0_Qa_02;
  float fVar15;
  double dVar16;
  undefined1 auStack_178 [32];
  undefined4 uStack_158;
  undefined4 uStack_150;
  undefined8 *puStack_148;
  undefined1 *puStack_140;
  undefined1 *puStack_138;
  uint uStack_130;
  undefined8 uStack_128;
  undefined8 uStack_120;
  undefined8 uStack_118;
  undefined1 uStack_110;
  undefined7 uStack_10f;
  ulonglong uStack_100;
  ulonglong uStack_f8;
  undefined1 uStack_f0;
  undefined7 uStack_ef;
  ulonglong uStack_e0;
  ulonglong uStack_d8;
  undefined1 uStack_d0;
  undefined7 uStack_cf;
  undefined8 uStack_c0;
  ulonglong uStack_b8;
  undefined8 auStack_b0 [3];
  ulonglong uStack_98;
  undefined8 auStack_90 [3];
  ulonglong uStack_78;
  undefined8 auStack_70 [3];
  ulonglong uStack_58;
  ulonglong uStack_50;
  
  uStack_118 = 0xfffffffffffffffe;
  uStack_50 = DAT_1802a5230 ^ (ulonglong)auStack_178;
  if (*(char *)(param_1 + 0x250) == '\0') goto LAB_1800a5db9;
  if ((*(int *)(param_1 + 0x22c) == 0) && (cVar4 = FUN_1800a6730(), cVar4 != '\0')) {
    cVar4 = FUN_18001dbc0(1);
    if (cVar4 != '\0') {
      uVar7 = FUN_18001db50();
      uStack_150 = *(undefined4 *)(param_1 + 0x22c);
      uStack_158 = 0x3cb;
      FUN_180015b40(uVar7,1,&DAT_180227e40,&DAT_180227d90);
    }
    goto LAB_1800a5db9;
  }
  bVar11 = false;
  iVar5 = *(int *)(param_1 + 0x248);
  if (iVar5 == 1) {
    uStack_128 = (undefined1 *)((ulonglong)uStack_128._4_4_ << 0x20);
    uStack_120 = (undefined1 *)((ulonglong)uStack_120._4_4_ << 0x20);
    iVar5 = FUN_1800a5e50(param_1,&uStack_128,&uStack_120);
    if (iVar5 == 0) {
      bVar11 = false;
      iVar5 = (int)(float)uStack_128;
      iVar3 = (int)(float)uStack_120;
      switch(*(undefined4 *)(param_1 + 0x24c)) {
      case 1:
        bVar11 = (int)(float)uStack_120 < (int)(float)uStack_128;
        break;
      case 2:
        bVar11 = (int)(float)uStack_128 < (int)(float)uStack_120;
        break;
      case 3:
        bVar11 = (float)uStack_128 == (float)uStack_120;
        break;
      case 4:
        bVar11 = (float)uStack_128 != (float)uStack_120;
        break;
      case 5:
        bVar11 = (int)(float)uStack_120 <= (int)(float)uStack_128;
        break;
      case 6:
        bVar11 = (int)(float)uStack_128 <= (int)(float)uStack_120;
      }
      cVar4 = FUN_18001dbc0(1);
      if (cVar4 != '\0') {
        puStack_148 = (undefined8 *)
                      FUN_1800a7000(extraout_XMM0_Qa_02,auStack_70,*(undefined4 *)(param_1 + 0x24c))
        ;
        if (0xf < (ulonglong)puStack_148[3]) {
          puStack_148 = (undefined8 *)*puStack_148;
        }
        uVar7 = FUN_18001db50();
        uStack_130 = (uint)bVar11;
        puStack_138 = (undefined1 *)CONCAT44(puStack_138._4_4_,iVar3);
        puStack_140 = (undefined1 *)CONCAT44(puStack_140._4_4_,iVar5);
        uStack_150 = *(undefined4 *)(param_1 + 0x24c);
        uStack_158 = 0x3df;
        FUN_180015b40(uVar7,1,&DAT_180227df0,&DAT_180227d90);
        if (0xf < uStack_58) {
          __3_YAXPEAX_Z(auStack_70[0]);
        }
      }
    }
    goto LAB_1800a5db9;
  }
  if (iVar5 == 2) {
    uStack_120 = (undefined1 *)((ulonglong)uStack_120._4_4_ << 0x20);
    uStack_128 = (undefined1 *)((ulonglong)uStack_128._4_4_ << 0x20);
    iVar5 = FUN_1800a6060(param_1,&uStack_120,&uStack_128);
    if (iVar5 != 0) goto LAB_1800a5db9;
    fVar2 = (float)uStack_120;
    fVar1 = (float)uStack_128;
    if (*(int *)(param_1 + 0x24c) == 3) {
      fVar15 = (float)(double)CONCAT44((uint)((ulonglong)
                                              (double)((float)uStack_120 - (float)uStack_128) >>
                                             0x20) & DAT_18023a820._4_4_,
                                       SUB84((double)((float)uStack_120 - (float)uStack_128),0) &
                                       (uint)DAT_18023a820);
      bVar11 = DAT_18023a710 == fVar15;
      bVar14 = DAT_18023a710 < fVar15;
      goto LAB_1800a5bfa;
    }
    bVar11 = false;
    switch(*(int *)(param_1 + 0x24c)) {
    case 1:
      bVar11 = (float)uStack_120 == (float)uStack_128;
      bVar14 = (float)uStack_120 < (float)uStack_128;
      goto LAB_1800a5bfa;
    case 2:
      bVar11 = (float)uStack_128 == (float)uStack_120;
      bVar14 = (float)uStack_128 < (float)uStack_120;
LAB_1800a5bfa:
      bVar11 = !bVar14 && !bVar11;
      break;
    case 3:
      if ((float)uStack_120 == (float)uStack_128) goto LAB_1800a5bb7;
LAB_1800a5bc3:
      bVar11 = false;
      break;
    case 4:
      if ((float)uStack_120 == (float)uStack_128) goto LAB_1800a5bc3;
LAB_1800a5bb7:
      bVar11 = true;
      break;
    case 5:
      bVar11 = (float)uStack_128 <= (float)uStack_120;
      break;
    case 6:
      bVar11 = (float)uStack_120 <= (float)uStack_128;
    }
    cVar4 = FUN_18001dbc0(1);
    if (cVar4 != '\0') {
      puStack_148 = (undefined8 *)
                    FUN_1800a7000(extraout_XMM0_Qa_01,auStack_90,*(undefined4 *)(param_1 + 0x24c));
      if (0xf < (ulonglong)puStack_148[3]) {
        puStack_148 = (undefined8 *)*puStack_148;
      }
      uVar7 = FUN_18001db50();
      uStack_130 = (uint)bVar11;
      uStack_150 = *(undefined4 *)(param_1 + 0x24c);
      uStack_158 = 0x3f4;
      puStack_140 = (undefined1 *)(double)fVar2;
      puStack_138 = (undefined1 *)(double)fVar1;
      FUN_180015b40(uVar7,1,&DAT_180227ed0,&DAT_180227d90);
      if (0xf < uStack_78) {
        __3_YAXPEAX_Z(auStack_90[0]);
      }
    }
    goto LAB_1800a5db9;
  }
  if (iVar5 != 3) {
    if (iVar5 != 7) goto LAB_1800a5db9;
    uStack_128 = (undefined1 *)0x0;
    uStack_120 = (undefined1 *)0x0;
    iVar5 = FUN_1800a6270(param_1,&uStack_128,&uStack_120);
    puVar8 = uStack_120;
    puVar9 = uStack_128;
    if (iVar5 != 0) goto LAB_1800a5db9;
    if (*(int *)(param_1 + 0x24c) == 3) {
      dVar16 = (double)CONCAT44((uint)((ulonglong)((double)uStack_128 - (double)uStack_120) >> 0x20)
                                & DAT_18023a820._4_4_,
                                SUB84((double)uStack_128 - (double)uStack_120,0) &
                                (uint)DAT_18023a820);
      bVar11 = DAT_18023a718 == dVar16;
      bVar14 = DAT_18023a718 < dVar16;
      goto LAB_1800a5736;
    }
    switch(*(int *)(param_1 + 0x24c)) {
    case 1:
      bVar11 = (double)uStack_128 == (double)uStack_120;
      bVar14 = (double)uStack_128 < (double)uStack_120;
      goto LAB_1800a5736;
    case 2:
      bVar11 = (double)uStack_120 == (double)uStack_128;
      bVar14 = (double)uStack_120 < (double)uStack_128;
LAB_1800a5736:
      bVar11 = !bVar14 && !bVar11;
      break;
    case 3:
      if ((double)uStack_128 == (double)uStack_120) goto LAB_1800a56f6;
LAB_1800a5703:
      bVar11 = false;
      break;
    case 4:
      if ((double)uStack_128 == (double)uStack_120) goto LAB_1800a5703;
LAB_1800a56f6:
      bVar11 = true;
      break;
    case 5:
      bVar11 = (double)uStack_120 <= (double)uStack_128;
      break;
    case 6:
      bVar11 = (double)uStack_128 <= (double)uStack_120;
    }
    cVar4 = FUN_18001dbc0(1);
    if (cVar4 != '\0') {
      puStack_148 = (undefined8 *)
                    FUN_1800a7000(extraout_XMM0_Qa,auStack_b0,*(undefined4 *)(param_1 + 0x24c));
      if (0xf < (ulonglong)puStack_148[3]) {
        puStack_148 = (undefined8 *)*puStack_148;
      }
      uVar7 = FUN_18001db50();
      uStack_130 = (uint)bVar11;
      puStack_138 = puVar8;
      puStack_140 = puVar9;
      uStack_150 = *(undefined4 *)(param_1 + 0x24c);
      uStack_158 = 0x409;
      FUN_180015b40(uVar7,1,&DAT_180227ed0,&DAT_180227d90);
      if (0xf < uStack_98) {
        __3_YAXPEAX_Z(auStack_b0[0]);
      }
    }
    goto LAB_1800a5db9;
  }
  uStack_d8 = 0xf;
  uStack_e0 = 0;
  uStack_f0 = 0;
  FUN_180006300(&uStack_f0,&DAT_18021cf35,0);
  uStack_f8 = 0xf;
  uStack_100 = 0;
  uStack_110 = 0;
  FUN_180006300(&uStack_110,&DAT_18021cf35,0);
  iVar5 = FUN_1800a6480(param_1,&uStack_f0,&uStack_110);
  uVar12 = uStack_e0;
  uVar10 = uStack_100;
  uStack_128 = (undefined1 *)CONCAT44(uStack_128._4_4_,iVar5);
  if (iVar5 != 0) goto LAB_1800a5b09;
  bVar11 = false;
  uVar6 = 0;
  switch(*(undefined4 *)(param_1 + 0x24c)) {
  case 1:
    puVar9 = &uStack_f0;
    if (0xf < uStack_d8) {
      puVar9 = (undefined1 *)CONCAT71(uStack_ef,uStack_f0);
    }
    puVar8 = &uStack_110;
    uVar10 = uStack_e0;
    uVar12 = uStack_100;
    if (0xf < uStack_f8) {
      puVar8 = (undefined1 *)CONCAT71(uStack_10f,uStack_110);
    }
    goto LAB_1800a5893;
  case 2:
    puVar9 = &uStack_110;
    if (0xf < uStack_f8) {
      puVar9 = (undefined1 *)CONCAT71(uStack_10f,uStack_110);
    }
    puVar8 = &uStack_f0;
    if (0xf < uStack_d8) {
      puVar8 = (undefined1 *)CONCAT71(uStack_ef,uStack_f0);
    }
LAB_1800a5893:
    uVar13 = uVar10;
    if (uVar12 < uVar10) {
      uVar13 = uVar12;
    }
    if (uVar13 != 0) {
      uVar6 = memcmp(puVar8,puVar9);
    }
    if (uVar6 == 0) {
      if (uVar12 < uVar10) {
        uVar6 = 0xffffffff;
      }
      else {
        uVar6 = (uint)(uVar12 != uVar10);
      }
    }
    bVar11 = (int)uVar6 < 0;
    break;
  case 3:
    puVar9 = &uStack_110;
    if (0xf < uStack_f8) {
      puVar9 = (undefined1 *)CONCAT71(uStack_10f,uStack_110);
    }
    puVar8 = &uStack_f0;
    if (0xf < uStack_d8) {
      puVar8 = (undefined1 *)CONCAT71(uStack_ef,uStack_f0);
    }
    uVar13 = uStack_100;
    if (uStack_e0 < uStack_100) {
      uVar13 = uStack_e0;
    }
    if (uVar13 != 0) {
      uVar6 = memcmp(puVar8,puVar9);
    }
    if (uVar6 == 0) {
      if (uVar12 < uVar10) {
        uVar6 = 0xffffffff;
      }
      else {
        uVar6 = (uint)(uVar12 != uVar10);
      }
    }
    bVar11 = uVar6 == 0;
    break;
  case 4:
    puVar9 = &uStack_110;
    if (0xf < uStack_f8) {
      puVar9 = (undefined1 *)CONCAT71(uStack_10f,uStack_110);
    }
    puVar8 = &uStack_f0;
    if (0xf < uStack_d8) {
      puVar8 = (undefined1 *)CONCAT71(uStack_ef,uStack_f0);
    }
    uVar13 = uStack_100;
    if (uStack_e0 < uStack_100) {
      uVar13 = uStack_e0;
    }
    if (uVar13 != 0) {
      uVar6 = memcmp(puVar8,puVar9);
    }
    if (uVar6 == 0) {
      if (uVar12 < uVar10) {
        uVar6 = 0xffffffff;
      }
      else {
        uVar6 = (uint)(uVar12 != uVar10);
      }
    }
    bVar11 = uVar6 != 0;
    break;
  case 5:
    puVar9 = &uStack_110;
    if (0xf < uStack_f8) {
      puVar9 = (undefined1 *)CONCAT71(uStack_10f,uStack_110);
    }
    puVar8 = &uStack_f0;
    if (0xf < uStack_d8) {
      puVar8 = (undefined1 *)CONCAT71(uStack_ef,uStack_f0);
    }
    goto LAB_1800a5a14;
  case 6:
    puVar9 = &uStack_f0;
    if (0xf < uStack_d8) {
      puVar9 = (undefined1 *)CONCAT71(uStack_ef,uStack_f0);
    }
    puVar8 = &uStack_110;
    uVar10 = uStack_e0;
    uVar12 = uStack_100;
    if (0xf < uStack_f8) {
      puVar8 = (undefined1 *)CONCAT71(uStack_10f,uStack_110);
    }
LAB_1800a5a14:
    uVar13 = uVar10;
    if (uVar12 < uVar10) {
      uVar13 = uVar12;
    }
    if (uVar13 != 0) {
      uVar6 = memcmp(puVar8,puVar9);
    }
    if (uVar6 == 0) {
      if (uVar12 < uVar10) {
        uVar6 = 0xffffffff;
      }
      else {
        uVar6 = (uint)(uVar12 != uVar10);
      }
    }
    bVar11 = -1 < (int)uVar6;
  }
  cVar4 = FUN_18001dbc0(1);
  if (cVar4 != '\0') {
    puStack_148 = (undefined8 *)
                  FUN_1800a7000(extraout_XMM0_Qa_00,&uStack_d0,*(undefined4 *)(param_1 + 0x24c));
    puStack_138 = &uStack_110;
    if (0xf < uStack_f8) {
      puStack_138 = (undefined1 *)CONCAT71(uStack_10f,uStack_110);
    }
    puVar9 = &uStack_f0;
    if (0xf < uStack_d8) {
      puVar9 = (undefined1 *)CONCAT71(uStack_ef,uStack_f0);
    }
    if (0xf < (ulonglong)puStack_148[3]) {
      puStack_148 = (undefined8 *)*puStack_148;
    }
    uVar7 = FUN_18001db50();
    uStack_130 = (uint)bVar11;
    uStack_150 = *(undefined4 *)(param_1 + 0x24c);
    uStack_158 = 0x416;
    puStack_140 = puVar9;
    FUN_180015b40(uVar7,1,&DAT_180227e80,&DAT_180227d90);
    if (0xf < uStack_b8) {
      __3_YAXPEAX_Z(CONCAT71(uStack_cf,uStack_d0));
    }
    uStack_b8 = 0xf;
    uStack_c0 = 0;
    uStack_d0 = 0;
  }
LAB_1800a5b09:
  if (0xf < uStack_f8) {
    __3_YAXPEAX_Z(CONCAT71(uStack_10f,uStack_110));
  }
  uStack_f8 = 0xf;
  uStack_100 = 0;
  uStack_110 = 0;
  if (0xf < uStack_d8) {
    __3_YAXPEAX_Z(CONCAT71(uStack_ef,uStack_f0));
  }
LAB_1800a5db9:
  FUN_1801e8810(uStack_50 ^ (ulonglong)auStack_178);
  return;
}

