
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800ffd00(undefined8 param_1,longlong *param_2,int param_3)

{
  int *piVar1;
  longlong *plVar2;
  int iVar3;
  undefined4 uVar4;
  char cVar5;
  char cVar6;
  int iVar7;
  undefined8 *puVar8;
  undefined8 uVar9;
  char *pcVar10;
  longlong *plVar11;
  undefined *puVar12;
  longlong *plVar13;
  longlong *plVar14;
  longlong lVar15;
  longlong lVar16;
  bool bVar17;
  undefined4 uVar18;
  undefined4 extraout_XMM0_Da;
  undefined1 auStack_238 [32];
  undefined4 uStack_218;
  int iStack_210;
  longlong *plStack_208;
  undefined *puStack_200;
  undefined *puStack_1f8;
  char cStack_1e8;
  int iStack_1e4;
  int iStack_1e0;
  longlong *plStack_1d8;
  int iStack_1d0;
  int iStack_1cc;
  longlong lStack_1c8;
  longlong *plStack_1c0;
  longlong lStack_1b8;
  longlong *plStack_1b0;
  longlong *plStack_1a8;
  undefined8 uStack_1a0;
  undefined8 uStack_198;
  longlong lStack_190;
  longlong *plStack_188;
  longlong lStack_180;
  longlong *plStack_178;
  longlong lStack_170;
  longlong *plStack_168;
  undefined4 *puStack_160;
  longlong *plStack_158;
  undefined8 uStack_150;
  undefined8 uStack_148;
  undefined8 uStack_140;
  undefined8 uStack_138;
  undefined8 uStack_130;
  undefined8 uStack_128;
  undefined1 auStack_120 [16];
  undefined8 uStack_110;
  undefined8 uStack_108;
  undefined1 auStack_100 [8];
  longlong *plStack_f8;
  undefined1 auStack_f0 [8];
  longlong *plStack_e8;
  undefined1 auStack_e0 [8];
  longlong *plStack_d8;
  undefined8 uStack_d0;
  undefined1 auStack_c8 [8];
  longlong *plStack_c0;
  longlong *plStack_b8;
  undefined1 auStack_b0 [8];
  longlong *plStack_a8;
  undefined1 uStack_a0;
  undefined7 uStack_9f;
  undefined8 uStack_90;
  ulonglong uStack_88;
  undefined1 uStack_80;
  undefined7 uStack_7f;
  undefined8 uStack_70;
  ulonglong uStack_68;
  undefined4 uStack_60;
  undefined4 uStack_5c;
  undefined4 uStack_58;
  undefined4 uStack_54;
  undefined4 uStack_50;
  undefined4 uStack_4c;
  undefined4 uStack_48;
  undefined4 uStack_44;
  undefined8 auStack_40 [3];
  ulonglong uStack_28;
  ulonglong uStack_20;
  
  uStack_d0 = 0xfffffffffffffffe;
  uStack_20 = _DAT_1802a5230 ^ (ulonglong)auStack_238;
  lVar15 = *param_2;
  plStack_1d8 = param_2;
  iStack_1d0 = param_3;
  uStack_198 = param_1;
  plStack_b8 = param_2;
  if (lVar15 == 0) {
    plVar13 = (longlong *)param_2[1];
    if (plVar13 != (longlong *)0x0) {
      LOCK();
      plVar11 = plVar13 + 1;
      *(int *)plVar11 = (int)*plVar11 + -1;
      UNLOCK();
      if ((int)*plVar11 == 0) {
        (**(code **)*plVar13)(plVar13);
        LOCK();
        piVar1 = (int *)((longlong)plVar13 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (*piVar1 == 0) {
          (**(code **)(*plVar13 + 8))(plVar13);
        }
      }
    }
    goto LAB_180100a84;
  }
  lVar16 = *(longlong *)(lVar15 + 0xf8);
  iStack_1cc = *(int *)(lVar15 + 0x30);
  iStack_1e4 = *(int *)(lVar15 + 0x2c);
  cStack_1e8 = '\0';
  lStack_1c8 = lVar16;
  if ((iStack_1cc - 3U & 0xfffffffd) == 0) {
    puVar8 = (undefined8 *)FUN_1800d33b0(auStack_100);
    FUN_1800d5770(*puVar8,iStack_1e4,lVar16);
    if (plStack_f8 != (longlong *)0x0) {
      LOCK();
      plVar13 = plStack_f8 + 1;
      *(int *)plVar13 = (int)*plVar13 + -1;
      UNLOCK();
      if ((int)*plVar13 == 0) {
        (**(code **)*plStack_f8)(plStack_f8);
        LOCK();
        piVar1 = (int *)((longlong)plStack_f8 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (*piVar1 == 0) {
          (**(code **)(*plStack_f8 + 8))(plStack_f8);
        }
      }
    }
    cStack_1e8 = '\x01';
  }
  if (*(char *)(*param_2 + 0x120) == '\0') {
    uVar9 = FUN_18008e6c0(lStack_1c8);
    FUN_1800cb9e0(uVar9,iStack_1e4);
  }
  lVar15 = *param_2;
  iVar7 = _Mtx_lock(lVar15 + 0x38);
  if (iVar7 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar7);
  }
  *(undefined4 *)(lVar15 + 0x20) = *(undefined4 *)(lVar15 + 0x24);
  iVar7 = _Mtx_unlock(lVar15 + 0x38);
  if (iVar7 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar7);
  }
  plVar13 = plStack_1d8;
  *(int *)(*plStack_1d8 + 0xec) = iStack_1d0;
  if ((_DAT_1802b2d98 & 1) == 0) {
    _DAT_1802b2d98 = _DAT_1802b2d98 | 1;
    FUN_1800d8c60();
    FUN_1801e8930(&UNK_180202bf0);
  }
  FUN_1800d9320(&DAT_1802b2da0,&puStack_160,iStack_1e4);
  if (puStack_160 != (undefined4 *)0x0) {
    _ftime64_s(&uStack_60);
    *puStack_160 = uStack_60;
    puStack_160[1] = uStack_5c;
    puStack_160[2] = uStack_58;
    puStack_160[3] = uStack_54;
    puStack_160[8] = 1;
  }
  plVar11 = plStack_158;
  if (plStack_158 != (longlong *)0x0) {
    LOCK();
    plVar14 = plStack_158 + 1;
    *(int *)plVar14 = (int)*plVar14 + -1;
    UNLOCK();
    if ((int)*plVar14 == 0) {
      (**(code **)*plStack_158)(plStack_158);
      LOCK();
      piVar1 = (int *)((longlong)plVar11 + 0xc);
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (*piVar1 == 0) {
        (**(code **)(*plStack_158 + 8))();
      }
    }
  }
  lVar15 = lStack_1c8;
  lStack_1b8 = 0;
  plStack_1b0 = (longlong *)0x0;
  if ((DAT_1802b129e == '\0') || (lStack_1c8 == 0)) {
    uVar18 = *(undefined4 *)(*plVar13 + 0x28);
    if ((_DAT_1802b1888 & 1) == 0) {
      _DAT_1802b1888 = _DAT_1802b1888 | 1;
      FUN_180107fc0();
      FUN_1801e8930(&UNK_180202980);
    }
    uVar9 = FUN_180108ba0(&DAT_1802b1890,auStack_f0,uVar18);
    uVar18 = FUN_18001d870(&lStack_1b8,uVar9);
    plVar11 = plStack_e8;
  }
  else {
    uVar9 = FUN_18008ede0();
    uVar9 = FUN_1800eab90(uVar9,auStack_b0,lVar15);
    uVar18 = FUN_18001d870(&lStack_1b8,uVar9);
    plVar11 = plStack_a8;
  }
  if (plVar11 != (longlong *)0x0) {
    LOCK();
    plVar14 = plVar11 + 1;
    *(int *)plVar14 = (int)*plVar14 + -1;
    UNLOCK();
    if ((int)*plVar14 == 0) {
      uVar18 = (**(code **)*plVar11)(plVar11);
      LOCK();
      piVar1 = (int *)((longlong)plVar11 + 0xc);
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (*piVar1 == 0) {
        uVar18 = (**(code **)(*plVar11 + 8))(plVar11);
      }
    }
  }
  lVar15 = lStack_1b8;
  iStack_1e0 = 3;
  if (((cStack_1e8 == '\0') || (plVar13 = plStack_1d8, lStack_1b8 == 0)) ||
     (cVar5 = FUN_1800e0cf0(lStack_1b8), plVar13 = plStack_1d8, uVar18 = extraout_XMM0_Da,
     cVar5 == '\0')) {
    if (*(char *)(*plVar13 + 0x120) == '\0') {
      if ((_DAT_1802b30c0 & 1) == 0) {
        _DAT_1802b30c0 = _DAT_1802b30c0 | 1;
        uVar18 = FUN_1801e8930(&UNK_180202f10);
      }
      iVar7 = FUN_1800b98c0(uVar18,iStack_1e4);
      plVar11 = (longlong *)*plVar13;
      if (iVar7 == 0) {
        iStack_1e0 = (**(code **)(*plVar11 + 8))();
      }
      else {
        if ((*(byte *)(plVar11 + 0xb) & 4) != 0) {
          iStack_1e0 = 2;
        }
        FUN_18001e4e0(plVar11,0);
        *(undefined1 *)(*plVar13 + 0x81) = 0;
        *(int *)(*plVar13 + 0x84) = iStack_1e0;
        *(int *)(*plVar13 + 0x7c) = iVar7;
        cVar5 = FUN_18001dbc0(1);
        if (cVar5 != '\0') {
          uVar9 = FUN_18001db50();
          plStack_208 = (longlong *)CONCAT44(plStack_208._4_4_,iVar7);
          iStack_210 = iStack_1e4;
          uStack_218 = 0x9d;
          FUN_180015b40(uVar9,1,&UNK_18022f860,&UNK_18022f838);
        }
      }
    }
    else {
      iStack_1e0 = *(int *)(*plVar13 + 0x84);
    }
  }
  else {
    *(undefined4 *)(*plStack_1d8 + 0x84) = 3;
    *(undefined4 *)(*plStack_1d8 + 0x7c) = 0xe0000009;
    cVar5 = FUN_18001dbc0(3);
    plVar13 = plStack_1d8;
    if (cVar5 != '\0') {
      uVar18 = *(undefined4 *)(lVar15 + 0x170);
      uVar4 = *(undefined4 *)(lVar15 + 0x50);
      uVar9 = FUN_18001db50();
      puStack_200 = (undefined *)CONCAT44(puStack_200._4_4_,uVar18);
      plStack_208 = (longlong *)CONCAT44(plStack_208._4_4_,uVar4);
      iStack_210 = iStack_1e4;
      uStack_218 = 0x85;
      FUN_180015b40(uVar9,3,&UNK_18022f8b0,&UNK_18022f838);
      plVar13 = plStack_1d8;
    }
  }
  uVar18 = *(undefined4 *)(*plVar13 + 0x2c);
  if ((_DAT_1802b2d98 & 1) == 0) {
    _DAT_1802b2d98 = _DAT_1802b2d98 | 1;
    FUN_1800d8c60();
    FUN_1801e8930(&UNK_180202bf0);
  }
  FUN_1800d9320(&DAT_1802b2da0,&lStack_180,uVar18);
  if (lStack_180 != 0) {
    _ftime64_s(&uStack_50);
    *(undefined4 *)(lStack_180 + 0x10) = uStack_50;
    *(undefined4 *)(lStack_180 + 0x14) = uStack_4c;
    *(undefined4 *)(lStack_180 + 0x18) = uStack_48;
    *(undefined4 *)(lStack_180 + 0x1c) = uStack_44;
    *(undefined4 *)(lStack_180 + 0x20) = 0;
  }
  plVar13 = plStack_178;
  if (plStack_178 != (longlong *)0x0) {
    LOCK();
    plVar11 = plStack_178 + 1;
    *(int *)plVar11 = (int)*plVar11 + -1;
    UNLOCK();
    if ((int)*plVar11 == 0) {
      (**(code **)*plStack_178)(plStack_178);
      LOCK();
      piVar1 = (int *)((longlong)plVar13 + 0xc);
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (*piVar1 == 0) {
        (**(code **)(*plStack_178 + 8))();
      }
    }
  }
  uVar18 = 1;
  if (iStack_1e0 == 0) {
    uVar18 = 0;
  }
  else if (iStack_1e0 == 1) {
    uVar18 = 3;
  }
  else if (iStack_1e0 == 2) {
    uVar18 = 2;
  }
  else if (iStack_1e0 == 5) {
    uVar18 = 5;
  }
  else if (iStack_1e0 == 6) {
    uVar18 = 6;
  }
  uVar4 = *(undefined4 *)(*plStack_1d8 + 0x7c);
  uVar9 = FUN_18008e6c0(lStack_1c8);
  FUN_1800cb8f0(uVar9,iStack_1e4,uVar18,uVar4);
  lVar15 = lStack_1c8;
  bVar17 = iStack_1d0 != 0;
  uVar9 = FUN_18008e6c0(lStack_1c8);
  FUN_1800cb970(uVar9,iStack_1e4,bVar17 + '\x01');
  plVar13 = plStack_1d8;
  pcVar10 = (char *)FUN_18001ed10(*plStack_1d8,auStack_40);
  if (0xf < *(ulonglong *)(pcVar10 + 0x18)) {
    pcVar10 = *(char **)pcVar10;
  }
  uStack_68 = 0xf;
  uStack_70 = 0;
  uStack_80 = 0;
  if (*pcVar10 == '\0') {
    lVar16 = 0;
  }
  else {
    lVar16 = -1;
    do {
      lVar16 = lVar16 + 1;
    } while (pcVar10[lVar16] != '\0');
  }
  FUN_180006300(&uStack_80,pcVar10,lVar16);
  uStack_88 = 0xf;
  uStack_90 = 0;
  uStack_a0 = 0;
  FUN_180006300(&uStack_a0,&UNK_18022cf68,0xe);
  uVar9 = FUN_18008e6c0(lVar15);
  FUN_1800cae00(uVar9,&lStack_190,iStack_1e4);
  if (lStack_190 == 0) {
    if (plStack_188 != (longlong *)0x0) {
      LOCK();
      plVar11 = plStack_188 + 1;
      *(int *)plVar11 = (int)*plVar11 + -1;
      iVar7 = (int)*plVar11;
      UNLOCK();
      plVar11 = plStack_188;
joined_r0x000180100376:
      if (iVar7 == 0) {
        plStack_188 = plVar11;
        (**(code **)*plVar11)(plVar11);
        LOCK();
        piVar1 = (int *)((longlong)plVar11 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        plVar13 = plStack_1d8;
        if (*piVar1 == 0) {
          (**(code **)(*plStack_188 + 8))();
          plVar13 = plStack_1d8;
        }
      }
    }
  }
  else {
    FUN_1800f7260(lStack_190,&lStack_170,&uStack_a0);
    if (lStack_170 != 0) {
      uStack_108 = 0xf;
      uStack_110 = 0;
      auStack_120[0] = 0;
      FUN_180006470(auStack_120,&uStack_80,0,0xffffffffffffffff);
      FUN_18000ffe0(lStack_170,0,auStack_120);
    }
    if (plStack_168 != (longlong *)0x0) {
      LOCK();
      plVar13 = plStack_168 + 1;
      *(int *)plVar13 = (int)*plVar13 + -1;
      UNLOCK();
      if ((int)*plVar13 == 0) {
        (**(code **)*plStack_168)(plStack_168);
        LOCK();
        piVar1 = (int *)((longlong)plStack_168 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (*piVar1 == 0) {
          (**(code **)(*plStack_168 + 8))(plStack_168);
        }
      }
    }
    plVar13 = plStack_1d8;
    if (plStack_188 != (longlong *)0x0) {
      LOCK();
      plVar11 = plStack_188 + 1;
      *(int *)plVar11 = (int)*plVar11 + -1;
      iVar7 = (int)*plVar11;
      UNLOCK();
      plVar11 = plStack_188;
      goto joined_r0x000180100376;
    }
  }
  if (0xf < uStack_88) {
    __3_YAXPEAX_Z(CONCAT71(uStack_9f,uStack_a0));
  }
  uStack_88 = 0xf;
  uStack_90 = 0;
  uStack_a0 = 0;
  if (0xf < uStack_68) {
    __3_YAXPEAX_Z(CONCAT71(uStack_7f,uStack_80));
  }
  uStack_68 = 0xf;
  uStack_70 = 0;
  uStack_80 = 0;
  if (0xf < uStack_28) {
    __3_YAXPEAX_Z(auStack_40[0]);
  }
  if (((iStack_1cc - 3U & 0xfffffffd) == 0) &&
     (((iStack_1e0 - 2U & 0xfffffff9) != 0 || (iStack_1e0 == 8)))) {
    if ((lStack_1b8 == 0) || (*(char *)(lStack_1b8 + 0x14c) != '\0')) {
      cVar5 = FUN_18001dbc0(1);
      if (cVar5 != '\0') {
        uVar18 = *(undefined4 *)(*plVar13 + 0x2c);
        iVar7 = *(int *)(*plVar13 + 0x28);
        uVar9 = FUN_18001db50();
        plStack_208 = (longlong *)CONCAT44(plStack_208._4_4_,uVar18);
        uStack_218 = 0xeb;
        iStack_210 = iVar7;
        FUN_180015b40(uVar9,1,&UNK_18022f910,&UNK_18022f838);
      }
    }
    else {
      uVar18 = *(undefined4 *)(*plVar13 + 0xf0);
      uVar4 = *(undefined4 *)(*plVar13 + 0x28);
      uVar9 = FUN_18008e6c0(lStack_1c8);
      uStack_218 = uVar18;
      FUN_1800ccfc0(uVar9,uVar4,iStack_1e4);
      plVar13 = plStack_1d8;
      iVar7 = iStack_1e4;
      if ((*(char *)(*plStack_1d8 + 0x80) == '\0') ||
         (pcVar10 = (char *)FUN_18008eae0(), iVar7 = iStack_1e4, *pcVar10 == '\0')) {
LAB_180100511:
        cVar5 = FUN_18001dbc0(1);
        lVar15 = lStack_1c8;
        if (cVar5 != '\0') {
          pcVar10 = (char *)FUN_18008eae0();
          cVar5 = *pcVar10;
          cStack_1e8 = *(char *)(*plVar13 + 0x80);
          uVar9 = FUN_18008eae0();
          cVar6 = FUN_1800ec610(uVar9,iVar7);
          plStack_1c0 = (longlong *)&UNK_18021cf28;
          puStack_1f8 = &UNK_18021cf28;
          if (cVar6 != '\0') {
            puStack_1f8 = &UNK_18021cf30;
          }
          puVar12 = &UNK_18021cf28;
          if (cVar5 != '\0') {
            puVar12 = &UNK_18021cf30;
          }
          if (cStack_1e8 != '\0') {
            plStack_1c0 = (longlong *)&UNK_18021cf30;
          }
          uVar9 = FUN_18001db50();
          plStack_208 = plStack_1c0;
          iStack_210 = iStack_1e4;
          uStack_218 = 0xd8;
          puStack_200 = puVar12;
          FUN_180015b40(uVar9,1,&UNK_18022f960,&UNK_18022f838);
          lVar15 = lStack_1c8;
        }
      }
      else {
        uVar9 = FUN_18008eae0();
        iVar7 = iStack_1e4;
        cVar5 = FUN_1800ec610(uVar9,iStack_1e4);
        if (cVar5 == '\0') goto LAB_180100511;
        puVar8 = (undefined8 *)FUN_1800d33b0(auStack_e0);
        lVar15 = lStack_1c8;
        FUN_1800d34a0(*puVar8,*(undefined4 *)(*plVar13 + 0x28),iStack_1e4,lStack_1c8);
        if (plStack_d8 != (longlong *)0x0) {
          LOCK();
          plVar13 = plStack_d8 + 1;
          *(int *)plVar13 = (int)*plVar13 + -1;
          UNLOCK();
          if ((int)*plVar13 == 0) {
            (**(code **)*plStack_d8)(plStack_d8);
            LOCK();
            piVar1 = (int *)((longlong)plStack_d8 + 0xc);
            *piVar1 = *piVar1 + -1;
            UNLOCK();
            if (*piVar1 == 0) {
              (**(code **)(*plStack_d8 + 8))(plStack_d8);
            }
          }
        }
      }
      if ((iStack_1cc == 5) && (iStack_1d0 == 1)) {
        puVar8 = (undefined8 *)FUN_1800d33b0(auStack_c8);
        iVar7 = iStack_1e4;
        FUN_1800d5770(*puVar8,iStack_1e4,lVar15);
        if (plStack_c0 != (longlong *)0x0) {
          LOCK();
          plVar13 = plStack_c0 + 1;
          lVar15 = *plVar13;
          *(int *)plVar13 = (int)*plVar13 + -1;
          UNLOCK();
          if ((int)lVar15 == 1) {
            (**(code **)*plStack_c0)(plStack_c0);
            LOCK();
            piVar1 = (int *)((longlong)plStack_c0 + 0xc);
            iVar3 = *piVar1;
            *piVar1 = *piVar1 + -1;
            UNLOCK();
            if (iVar3 == 1) {
              (**(code **)(*plStack_c0 + 8))(plStack_c0);
            }
          }
        }
        if (100000 < iVar7) {
          FUN_1800df5c0(lStack_1b8,iVar7);
        }
        FUN_1800df3c0(lStack_1b8,iVar7);
      }
    }
  }
  iVar7 = iStack_1e0;
  FUN_18001e310(*plStack_1d8,&plStack_1a8);
  plVar13 = (longlong *)*plStack_1a8;
  plVar11 = plStack_1a8;
  if (plVar13 != plStack_1a8) {
    do {
      plVar14 = plVar11;
      if (plVar13[2] == 0) {
        cVar5 = FUN_18001dbc0(4);
        if (cVar5 != '\0') {
          uVar9 = FUN_18001db50();
          uStack_218 = 0xf8;
          FUN_180015b40(uVar9,4,&UNK_18022fa18,&UNK_18022f838);
        }
      }
      else {
        switch(iVar7) {
        case 0:
        case 4:
        case 5:
        case 6:
          FUN_18001e400(plVar13[2],1);
          break;
        case 1:
        case 3:
          cVar5 = FUN_18001dbc0(1);
          if (cVar5 != '\0') {
            uVar18 = *(undefined4 *)(plVar13[2] + 0x2c);
            uVar9 = FUN_18001db50();
            plStack_208 = (longlong *)CONCAT44(plStack_208._4_4_,uVar18);
            iStack_210 = iStack_1e4;
            uStack_218 = 0x10a;
            FUN_180015b40(uVar9,1,&UNK_18022f9d0,&UNK_18022f838);
          }
          FUN_18001e400(plVar13[2],2);
          break;
        case 2:
          cVar5 = FUN_18001dbc0(1);
          if (cVar5 != '\0') {
            uVar18 = *(undefined4 *)(plVar13[2] + 0x2c);
            uVar9 = FUN_18001db50();
            plStack_208 = (longlong *)CONCAT44(plStack_208._4_4_,uVar18);
            iStack_210 = iStack_1e4;
            uStack_218 = 0x110;
            FUN_180015b40(uVar9,1,&UNK_18022fa68,&UNK_18022f838);
          }
          FUN_18001e400(plVar13[2],4);
          *(undefined4 *)(plVar13[2] + 0xe8) = *(undefined4 *)(*plStack_1d8 + 0xe8);
          break;
        default:
          cVar5 = FUN_18001dbc0(4);
          if (cVar5 != '\0') {
            uVar9 = FUN_18001db50();
            uStack_218 = 0x117;
            iStack_210 = iVar7;
            FUN_180015b40(uVar9,4,&UNK_18022fa40,&UNK_18022f838);
          }
        }
        plStack_1c0 = (longlong *)plVar13[2];
        plVar2 = plStack_1c0 + 7;
        iVar7 = _Mtx_lock(plVar2);
        if (iVar7 != 0) {
          __Throw_C_error_std__YAXH_Z(iVar7);
        }
        if (0 < (int)plStack_1c0[4]) {
          *(int *)(plStack_1c0 + 4) = (int)plStack_1c0[4] + -1;
        }
        iStack_1cc = (int)plStack_1c0[4];
        iVar7 = _Mtx_unlock(plVar2);
        if (iVar7 != 0) {
          __Throw_C_error_std__YAXH_Z(iVar7);
        }
        iVar7 = iStack_1d0;
        if (iStack_1cc == 0) {
          *(int *)(plVar13[2] + 0xec) = iStack_1d0;
          lVar15 = plVar13[2];
          if ((plVar11 != plStack_1a8) || (plVar14 = plVar13, *(int *)(lVar15 + 0x30) == 5)) {
            lVar16 = plVar13[3];
            if (*(int *)(lVar15 + 0x30) == 2) {
              uStack_140 = 0;
              uStack_138 = 0;
              if (lVar16 != 0) {
                LOCK();
                *(int *)(lVar16 + 8) = *(int *)(lVar16 + 8) + 1;
                UNLOCK();
              }
              FUN_180015c40(&uStack_140,lVar15);
              FUN_1800ffd00(uStack_198,&uStack_140,iVar7);
              plVar14 = plVar11;
            }
            else {
              uStack_150 = 0;
              uStack_148 = 0;
              if (lVar16 != 0) {
                LOCK();
                *(int *)(lVar16 + 8) = *(int *)(lVar16 + 8) + 1;
                UNLOCK();
              }
              FUN_180015c40(&uStack_150,lVar15);
              FUN_1800ffaa0(uStack_198,&uStack_150,iVar7);
              plVar14 = plVar11;
            }
          }
        }
      }
      plVar13 = (longlong *)*plVar13;
      plVar11 = plVar14;
      iVar7 = iStack_1e0;
    } while (plVar13 != plStack_1a8);
    if (plVar14 != plStack_1a8) {
      uStack_130 = 0;
      uStack_128 = 0;
      lVar15 = plVar14[2];
      if (plVar14[3] != 0) {
        LOCK();
        piVar1 = (int *)(plVar14[3] + 8);
        *piVar1 = *piVar1 + 1;
        UNLOCK();
      }
      FUN_180015c40(&uStack_130,lVar15);
      FUN_1800ffd00(uStack_198,&uStack_130,iStack_1d0);
    }
  }
  plVar13 = (longlong *)*plStack_1a8;
  *plStack_1a8 = (longlong)plStack_1a8;
  plStack_1a8[1] = (longlong)plStack_1a8;
  uStack_1a0 = 0;
  if (plVar13 != plStack_1a8) {
    do {
      plStack_1c0 = (longlong *)*plVar13;
      plVar11 = (longlong *)plVar13[3];
      if (plVar11 != (longlong *)0x0) {
        LOCK();
        plVar14 = plVar11 + 1;
        *(int *)plVar14 = (int)*plVar14 + -1;
        UNLOCK();
        if ((int)*plVar14 == 0) {
          (**(code **)*plVar11)(plVar11);
          LOCK();
          piVar1 = (int *)((longlong)plVar11 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (*piVar1 == 0) {
            (**(code **)(*plVar11 + 8))(plVar11);
          }
        }
      }
      __3_YAXPEAX_Z(plVar13);
      plVar13 = plStack_1c0;
    } while (plStack_1c0 != plStack_1a8);
  }
  __3_YAXPEAX_Z();
  plVar13 = plStack_1b0;
  if (plStack_1b0 != (longlong *)0x0) {
    LOCK();
    plVar11 = plStack_1b0 + 1;
    lVar15 = *plVar11;
    *(int *)plVar11 = (int)*plVar11 + -1;
    UNLOCK();
    if ((int)lVar15 == 1) {
      (**(code **)*plStack_1b0)(plStack_1b0);
      LOCK();
      piVar1 = (int *)((longlong)plVar13 + 0xc);
      iVar7 = *piVar1;
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (iVar7 == 1) {
        (**(code **)(*plStack_1b0 + 8))();
      }
    }
  }
  plVar13 = (longlong *)plStack_1d8[1];
  if (plVar13 != (longlong *)0x0) {
    LOCK();
    plVar11 = plVar13 + 1;
    lVar15 = *plVar11;
    *(int *)plVar11 = (int)*plVar11 + -1;
    UNLOCK();
    if ((int)lVar15 == 1) {
      (**(code **)*plVar13)(plVar13);
      LOCK();
      piVar1 = (int *)((longlong)plVar13 + 0xc);
      iVar7 = *piVar1;
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (iVar7 == 1) {
        (**(code **)(*plVar13 + 8))(plVar13);
      }
    }
  }
LAB_180100a84:
  FUN_1801e8810(uStack_20 ^ (ulonglong)auStack_238);
  return;
}

