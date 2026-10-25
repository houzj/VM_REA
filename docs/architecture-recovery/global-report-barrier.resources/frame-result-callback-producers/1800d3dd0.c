
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800d3dd0(longlong param_1)

{
  int *piVar1;
  undefined4 uVar2;
  double dVar3;
  double dVar4;
  double dVar5;
  longlong *plVar6;
  char cVar7;
  uint uVar8;
  int iVar9;
  int iVar10;
  longlong lVar11;
  undefined8 uVar12;
  undefined8 *puVar13;
  undefined8 *puVar14;
  undefined8 *puVar15;
  undefined8 *puVar16;
  longlong *plVar17;
  undefined8 *puVar18;
  undefined8 *puVar19;
  ulonglong uVar20;
  undefined1 auStack_1a8 [32];
  longlong lStack_188;
  int iStack_180;
  ulonglong uStack_178;
  undefined8 *puStack_170;
  double dStack_168;
  int aiStack_158 [2];
  ulonglong uStack_150;
  int aiStack_148 [2];
  int iStack_140;
  undefined4 uStack_13c;
  undefined4 uStack_138;
  undefined4 uStack_134;
  undefined8 uStack_130;
  longlong *plStack_128;
  int *piStack_120;
  int *piStack_118;
  undefined8 *puStack_110;
  longlong lStack_108;
  int *piStack_100;
  int *piStack_f8;
  undefined8 *puStack_f0;
  int *piStack_e8;
  undefined8 *puStack_e0;
  longlong lStack_d8;
  undefined8 *puStack_d0;
  longlong lStack_c8;
  int *piStack_c0;
  undefined8 *puStack_b8;
  undefined8 *puStack_b0;
  longlong lStack_a8;
  undefined8 uStack_a0;
  undefined8 uStack_98;
  longlong lStack_90;
  undefined1 auStack_88 [8];
  undefined8 auStack_80 [3];
  ulonglong uStack_68;
  ulonglong uStack_60;
  
  dVar5 = _DAT_18023a7b0;
  uStack_98 = 0xfffffffffffffffe;
  uStack_60 = _DAT_1802a5230 ^ (ulonglong)auStack_1a8;
  while ((uVar8 = WaitForSingleObject(*(undefined8 *)(param_1 + 8),0), (uVar8 & 0xffffff7f) != 0 &&
         ((uVar8 == 0x102 || (iVar9 = GetLastError(), iVar9 != 0))))) {
    iVar9 = WaitForSingleObject(*(undefined8 *)(param_1 + 0x10));
    if ((iVar9 != 0) && ((iVar9 != 0x80 && (iVar9 != 0x102)))) {
      GetLastError();
    }
    aiStack_158[0] = -1;
    uStack_150 = 0;
    FUN_1801e5430(param_1);
    if (*(longlong *)(param_1 + 0x58) == 0) {
      iVar9 = ResetEvent(*(undefined8 *)(param_1 + 0x10));
      if (iVar9 == 0) {
        GetLastError();
      }
      FUN_1801e5390(param_1);
    }
    else {
      aiStack_158[0] = *(int *)(**(longlong **)(param_1 + 0x50) + 0x10);
      uStack_150 = *(ulonglong *)(**(longlong **)(param_1 + 0x50) + 0x18);
      plVar17 = (longlong *)**(longlong **)(param_1 + 0x50);
      *(longlong *)plVar17[1] = *plVar17;
      *(longlong *)(*plVar17 + 8) = plVar17[1];
      *(longlong *)(param_1 + 0x58) = *(longlong *)(param_1 + 0x58) + -1;
      __3_YAXPEAX_Z();
      FUN_1801e5390(param_1);
      uVar20 = uStack_150;
      iVar9 = aiStack_158[0];
      uStack_130 = 0;
      plStack_128 = (longlong *)0x0;
      aiStack_148[0] = aiStack_158[0];
      lVar11 = FUN_18008e6c0(uStack_150);
      plVar17 = (longlong *)(lVar11 + 0x60);
      puVar13 = (undefined8 *)*plVar17;
      cVar7 = *(char *)((longlong)puVar13[1] + 0x19);
      puVar18 = puVar13;
      puVar16 = (undefined8 *)puVar13[1];
      while (cVar7 == '\0') {
        if (*(int *)(puVar16 + 4) < aiStack_148[0]) {
          puVar15 = (undefined8 *)puVar16[2];
          puVar16 = puVar18;
        }
        else {
          puVar15 = (undefined8 *)*puVar16;
        }
        puVar18 = puVar16;
        puVar16 = puVar15;
        cVar7 = *(char *)((longlong)puVar15 + 0x19);
      }
      if ((puVar18 == puVar13) || (aiStack_148[0] < *(int *)(puVar18 + 4))) {
        piStack_c0 = aiStack_148;
        lStack_188 = FUN_1800d0f90(plVar17,aiStack_148[0],&piStack_c0);
        FUN_1800d1010(plVar17,&puStack_b0,puVar18,lStack_188 + 0x20);
        puVar18 = puStack_b0;
      }
      if (puVar18[6] == 0) {
        cVar7 = FUN_18001dbc0(4);
        plVar17 = plStack_128;
        if (cVar7 != '\0') {
          puVar13 = (undefined8 *)FUN_18008f750(auStack_80,0xe0000003);
          if (0xf < (ulonglong)puVar13[3]) {
            puVar13 = (undefined8 *)*puVar13;
          }
          uVar12 = FUN_18001db50();
          uStack_178 = uVar20;
          iStack_180 = iVar9;
          lStack_188 = CONCAT44(lStack_188._4_4_,0x12d);
          puStack_170 = puVar13;
          FUN_180015b40(uVar12,4,&UNK_18022be60,&UNK_18022be00);
          plVar17 = plStack_128;
          if (0xf < uStack_68) {
            __3_YAXPEAX_Z(auStack_80[0]);
            plVar17 = plStack_128;
          }
        }
      }
      else {
        lVar11 = *(longlong *)puVar18[5];
        uVar12 = *(undefined8 *)(lVar11 + 0x30);
        uVar2 = *(undefined4 *)(lVar11 + 0x20);
        plVar17 = *(longlong **)(lVar11 + 0x18);
        uStack_130 = *(undefined8 *)(lVar11 + 0x10);
        if (plVar17 != (longlong *)0x0) {
          LOCK();
          *(int *)(plVar17 + 1) = (int)plVar17[1] + 1;
          UNLOCK();
        }
        plStack_128 = plVar17;
        uStack_a0 = uVar12;
        iVar10 = QueryPerformanceFrequency(&lStack_c8);
        dVar4 = 0.0;
        if ((iVar10 != 0) &&
           (iVar10 = QueryPerformanceCounter(&lStack_d8), dVar4 = 0.0, iVar10 != 0)) {
          dVar4 = ((double)lStack_d8 / (double)lStack_c8) * dVar5;
        }
        if (_DAT_1802a5260 != (code *)0x0) {
          (*_DAT_1802a5260)(uVar12,uVar2,_DAT_1802b1188);
        }
        cVar7 = FUN_18001dbc0(2);
        if (cVar7 != '\0') {
          iVar10 = QueryPerformanceFrequency(&lStack_a8);
          dVar3 = 0.0;
          if ((iVar10 != 0) &&
             (iVar10 = QueryPerformanceCounter(&lStack_108), dVar3 = 0.0, iVar10 != 0)) {
            dVar3 = ((double)lStack_108 / (double)lStack_a8) * dVar5;
          }
          uVar12 = FUN_18001db50();
          puStack_170 = (undefined8 *)CONCAT44(puStack_170._4_4_,uVar2);
          uStack_178 = uVar20;
          iStack_180 = iVar9;
          lStack_188 = CONCAT44(lStack_188._4_4_,0x129);
          dStack_168 = dVar3 - dVar4;
          FUN_180015b40(uVar12,2,&UNK_18022bda0,&UNK_18022be00);
        }
      }
      plVar6 = plStack_128;
      if (plVar17 != (longlong *)0x0) {
        LOCK();
        plVar17 = plVar17 + 1;
        *(int *)plVar17 = (int)*plVar17 + -1;
        UNLOCK();
        if ((int)*plVar17 == 0) {
          (**(code **)*plStack_128)(plStack_128);
          LOCK();
          piVar1 = (int *)((longlong)plVar6 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (*piVar1 == 0) {
            (**(code **)(*plStack_128 + 8))();
          }
        }
      }
      lStack_90 = param_1;
      FUN_1801e5430(param_1);
      puVar13 = *(undefined8 **)(param_1 + 0x70);
      cVar7 = *(char *)((longlong)puVar13[1] + 0x19);
      puVar18 = puVar13;
      puVar16 = (undefined8 *)puVar13[1];
      while (cVar7 == '\0') {
        if ((*(int *)(puVar16 + 4) < aiStack_158[0]) ||
           ((*(int *)(puVar16 + 4) <= aiStack_158[0] && ((ulonglong)puVar16[5] < uStack_150)))) {
          puVar15 = (undefined8 *)puVar16[2];
        }
        else {
          puVar15 = (undefined8 *)*puVar16;
          puVar18 = puVar16;
        }
        puVar16 = puVar15;
        cVar7 = *(char *)((longlong)puVar15 + 0x19);
      }
      if (((puVar18 == puVar13) || (aiStack_158[0] < *(int *)(puVar18 + 4))) ||
         ((aiStack_158[0] <= *(int *)(puVar18 + 4) && (uStack_150 < (ulonglong)puVar18[5])))) {
        piStack_f8 = aiStack_158;
        lStack_188 = FUN_1800d7480(param_1 + 0x70,aiStack_158[0],&piStack_f8);
        FUN_1800d74c0(param_1 + 0x70,&puStack_e0,puVar18,lStack_188 + 0x20);
        puVar18 = puStack_e0;
      }
      iStack_140 = *(int *)(puVar18 + 6);
      uStack_13c = *(undefined4 *)((longlong)puVar18 + 0x34);
      uStack_138 = (undefined4)puVar18[7];
      uStack_134 = *(undefined4 *)((longlong)puVar18 + 0x3c);
      uVar20 = puVar18[7];
      puVar13 = *(undefined8 **)(param_1 + 0x60);
      cVar7 = *(char *)((longlong)puVar13[1] + 0x19);
      puVar18 = puVar13;
      puVar16 = (undefined8 *)puVar13[1];
      while (cVar7 == '\0') {
        if ((*(int *)(puVar16 + 4) < iStack_140) ||
           ((*(int *)(puVar16 + 4) <= iStack_140 && ((ulonglong)puVar16[5] < uVar20)))) {
          puVar15 = (undefined8 *)puVar16[2];
        }
        else {
          puVar15 = (undefined8 *)*puVar16;
          puVar18 = puVar16;
        }
        puVar16 = puVar15;
        cVar7 = *(char *)((longlong)puVar15 + 0x19);
      }
      if (((puVar18 == puVar13) || (iStack_140 < *(int *)(puVar18 + 4))) ||
         ((iStack_140 <= *(int *)(puVar18 + 4) && (uVar20 < (ulonglong)puVar18[5])))) {
        piStack_e8 = &iStack_140;
        lStack_188 = FUN_1800d7400(param_1 + 0x60,aiStack_158[0],&piStack_e8);
        FUN_1800d7150(param_1 + 0x60,&puStack_d0,puVar18,lStack_188 + 0x20);
        puVar18 = puStack_d0;
      }
      puVar13 = (undefined8 *)puVar18[6];
      puVar16 = (undefined8 *)puVar13[1];
      cVar7 = *(char *)((longlong)puVar16 + 0x19);
      puVar15 = puVar13;
      puVar19 = puVar16;
      while (cVar7 == '\0') {
        iVar9 = *(int *)(puVar19 + 4);
        if ((iVar9 < aiStack_158[0]) ||
           ((iVar9 <= aiStack_158[0] && ((ulonglong)puVar19[5] < uStack_150)))) {
          puVar14 = (undefined8 *)puVar19[2];
        }
        else {
          if ((*(char *)((longlong)puVar13 + 0x19) != '\0') &&
             ((aiStack_158[0] < iVar9 ||
              ((aiStack_158[0] <= iVar9 && (uStack_150 < (ulonglong)puVar19[5])))))) {
            puVar13 = puVar19;
          }
          puVar14 = (undefined8 *)*puVar19;
          puVar15 = puVar19;
        }
        puVar19 = puVar14;
        cVar7 = *(char *)((longlong)puVar14 + 0x19);
      }
      if (*(char *)((longlong)puVar13 + 0x19) == '\0') {
        puVar16 = (undefined8 *)*puVar13;
      }
      cVar7 = *(char *)((longlong)puVar16 + 0x19);
      while (puVar19 = puVar16, puVar16 = puVar15, cVar7 == '\0') {
        if ((aiStack_158[0] < *(int *)(puVar19 + 4)) ||
           ((aiStack_158[0] <= *(int *)(puVar19 + 4) && (uStack_150 < (ulonglong)puVar19[5])))) {
          puVar16 = (undefined8 *)*puVar19;
        }
        else {
          puVar16 = (undefined8 *)puVar19[2];
          puVar19 = puVar13;
        }
        cVar7 = *(char *)((longlong)puVar16 + 0x19);
        puVar13 = puVar19;
      }
      while (puVar16 != puVar13) {
        if (*(char *)((longlong)puVar16 + 0x19) == '\0') {
          puVar19 = (undefined8 *)puVar16[2];
          if (*(char *)((longlong)puVar19 + 0x19) == '\0') {
            cVar7 = *(char *)((longlong)*puVar19 + 0x19);
            puVar16 = puVar19;
            puVar19 = (undefined8 *)*puVar19;
            while (cVar7 == '\0') {
              cVar7 = *(char *)((longlong)*puVar19 + 0x19);
              puVar16 = puVar19;
              puVar19 = (undefined8 *)*puVar19;
            }
          }
          else {
            cVar7 = *(char *)((longlong)puVar16[1] + 0x19);
            puVar14 = (undefined8 *)puVar16[1];
            puVar19 = puVar16;
            while ((puVar16 = puVar14, cVar7 == '\0' && (puVar19 == (undefined8 *)puVar16[2]))) {
              cVar7 = *(char *)((longlong)puVar16[1] + 0x19);
              puVar14 = (undefined8 *)puVar16[1];
              puVar19 = puVar16;
            }
          }
        }
      }
      FUN_180092540(puVar18 + 6,auStack_88,puVar15);
      puVar13 = *(undefined8 **)(param_1 + 0x60);
      uVar20 = CONCAT44(uStack_134,uStack_138);
      cVar7 = *(char *)((longlong)puVar13[1] + 0x19);
      puVar18 = puVar13;
      puVar16 = (undefined8 *)puVar13[1];
      while (cVar7 == '\0') {
        if ((*(int *)(puVar16 + 4) < iStack_140) ||
           ((*(int *)(puVar16 + 4) <= iStack_140 && ((ulonglong)puVar16[5] < uVar20)))) {
          puVar15 = (undefined8 *)puVar16[2];
        }
        else {
          puVar15 = (undefined8 *)*puVar16;
          puVar18 = puVar16;
        }
        puVar16 = puVar15;
        cVar7 = *(char *)((longlong)puVar15 + 0x19);
      }
      if (((puVar18 == puVar13) || (iStack_140 < *(int *)(puVar18 + 4))) ||
         ((iStack_140 <= *(int *)(puVar18 + 4) && (uVar20 < (ulonglong)puVar18[5])))) {
        piStack_120 = &iStack_140;
        lStack_188 = FUN_1800d7400(param_1 + 0x60,iStack_140,&piStack_120);
        FUN_1800d7150(param_1 + 0x60,&puStack_b8,puVar18,lStack_188 + 0x20);
        uVar20 = CONCAT44(uStack_134,uStack_138);
        puVar18 = puStack_b8;
      }
      if (puVar18[7] == 0) {
        puVar13 = *(undefined8 **)(param_1 + 0x30);
        cVar7 = *(char *)((longlong)puVar13[1] + 0x19);
        puVar18 = puVar13;
        puVar16 = (undefined8 *)puVar13[1];
        while (cVar7 == '\0') {
          if ((*(int *)(puVar16 + 4) < iStack_140) ||
             ((*(int *)(puVar16 + 4) <= iStack_140 && ((ulonglong)puVar16[5] < uVar20)))) {
            puVar15 = (undefined8 *)puVar16[2];
          }
          else {
            puVar15 = (undefined8 *)*puVar16;
            puVar18 = puVar16;
          }
          puVar16 = puVar15;
          cVar7 = *(char *)((longlong)puVar15 + 0x19);
        }
        if (((puVar18 == puVar13) || (iStack_140 < *(int *)(puVar18 + 4))) ||
           ((iStack_140 <= *(int *)(puVar18 + 4) && (uVar20 < (ulonglong)puVar18[5])))) {
          piStack_118 = &iStack_140;
          lStack_188 = FUN_1800d6db0(param_1 + 0x30,iStack_140,&piStack_118);
          FUN_1800d6df0(param_1 + 0x30,&puStack_110,puVar18,lStack_188 + 0x20);
          puVar18 = puStack_110;
        }
        iVar9 = SetEvent(puVar18[6]);
        if (iVar9 == 0) {
          GetLastError();
        }
      }
      FUN_1801e5390(param_1);
      puVar13 = *(undefined8 **)(param_1 + 0x20);
      cVar7 = *(char *)((longlong)puVar13[1] + 0x19);
      puVar18 = puVar13;
      puVar16 = (undefined8 *)puVar13[1];
      while (cVar7 == '\0') {
        if ((*(int *)(puVar16 + 4) < aiStack_158[0]) ||
           ((*(int *)(puVar16 + 4) <= aiStack_158[0] && ((ulonglong)puVar16[5] < uStack_150)))) {
          puVar15 = (undefined8 *)puVar16[2];
        }
        else {
          puVar15 = (undefined8 *)*puVar16;
          puVar18 = puVar16;
        }
        puVar16 = puVar15;
        cVar7 = *(char *)((longlong)puVar15 + 0x19);
      }
      if (((puVar18 == puVar13) || (aiStack_158[0] < *(int *)(puVar18 + 4))) ||
         ((aiStack_158[0] <= *(int *)(puVar18 + 4) && (uStack_150 < (ulonglong)puVar18[5])))) {
        piStack_100 = aiStack_158;
        lStack_188 = FUN_1800d6db0(param_1 + 0x20,aiStack_158[0],&piStack_100);
        FUN_1800d6df0(param_1 + 0x20,&puStack_f0,puVar18,lStack_188 + 0x20);
        puVar18 = puStack_f0;
      }
      iVar9 = SetEvent(puVar18[6]);
      if (iVar9 == 0) {
        GetLastError();
      }
    }
  }
  FUN_1801e8810(uStack_60 ^ (ulonglong)auStack_1a8);
  return;
}

