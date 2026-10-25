
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined8 IMVS_ReleaseDynamicMemory(longlong param_1,uint param_2,longlong *param_3)

{
  undefined8 *puVar1;
  undefined8 *puVar2;
  undefined8 *puVar3;
  char cVar4;
  longlong *plVar5;
  undefined8 uVar6;
  ulonglong uVar7;
  longlong *plVar8;
  ulonglong uVar9;
  ulonglong uVar10;
  uint uVar11;
  ulonglong uVar12;
  
                    /* 0x650730  633  IMVS_ReleaseDynamicMemory */
  plVar5 = (longlong *)FUN_180645170();
  if (plVar5 == (longlong *)0x0) {
    return 0xe0000006;
  }
  puVar1 = (undefined8 *)*plVar5;
  puVar2 = (undefined8 *)*puVar1;
  puVar3 = puVar2;
  while( true ) {
    if (puVar3 == puVar1) {
      return 0xe0000006;
    }
    if (puVar3[2] == param_1) break;
    puVar3 = (undefined8 *)*puVar3;
  }
  if (puVar3[3] == 0) {
    return 0xe0000006;
  }
  while( true ) {
    if (puVar2 == puVar1) {
      return 0xe0000006;
    }
    if (puVar2[3] == puVar3[3]) break;
    puVar2 = (undefined8 *)*puVar2;
  }
  if (puVar2[2] == 0) {
    return 0xe0000006;
  }
  if (puVar2[2] != param_1) {
    return 0xe0000006;
  }
  if ((_DAT_180ca02d8 & 1) == 0) {
    _DAT_180ca02d8 = _DAT_180ca02d8 | 1;
    FUN_180660680();
    FUN_1809e7f58(&UNK_180a2a8b0);
  }
  cVar4 = FUN_1805a2010(2);
  if (cVar4 != '\0') {
    uVar6 = FUN_1805a1d70();
    FUN_1805a1f10(uVar6,2,&UNK_180abc4d0,&UNK_180ad10e8,0x4e59);
  }
  if (param_3 == (longlong *)0x0) {
    return 0;
  }
  if (10 < param_2) {
    return 0xe0000007;
  }
  switch(param_2) {
  case 0:
    free(param_3);
    goto code_r0x0001806af28d;
  case 1:
    break;
  case 2:
    ___V_YAXPEAX_Z(param_3);
    goto code_r0x0001806af28d;
  case 3:
    if (param_3[1] != 0) {
      uVar7 = 0;
      uVar9 = uVar7;
      if (0 < (int)*param_3) {
        do {
          if (*(longlong *)(uVar7 + 0x80 + param_3[1]) != 0) {
            ___V_YAXPEAX_Z();
            *(undefined8 *)(uVar7 + 0x80 + param_3[1]) = 0;
          }
          *(undefined4 *)(uVar7 + 0x88 + param_3[1]) = 0;
          memset((longlong)(int)uVar9 * 0x90 + param_3[1],0,0x80);
          uVar11 = (int)uVar9 + 1;
          uVar7 = uVar7 + 0x90;
          uVar9 = (ulonglong)uVar11;
        } while ((int)uVar11 < (int)*param_3);
      }
      *(int *)param_3 = 0;
      ___V_YAXPEAX_Z(param_3[1]);
      param_3[1] = 0;
    }
    break;
  case 4:
    uVar7 = 0;
    uVar9 = uVar7;
    plVar5 = param_3;
    if (0 < (int)*param_3) {
      do {
        plVar8 = plVar5 + 2;
        if (*plVar8 != 0) {
          uVar10 = uVar7;
          uVar12 = uVar7;
          if (0 < *(int *)((longlong)plVar5 + 0xc)) {
            do {
              if (*(longlong *)(uVar10 + 0x80 + *plVar8) != 0) {
                ___V_YAXPEAX_Z();
                *(undefined8 *)(uVar10 + 0x80 + *plVar8) = 0;
              }
              uVar11 = (int)uVar12 + 1;
              uVar10 = uVar10 + 0x90;
              uVar12 = (ulonglong)uVar11;
            } while ((int)uVar11 < *(int *)((longlong)plVar5 + 0xc));
          }
          ___V_YAXPEAX_Z(*plVar8);
          *plVar8 = 0;
          *(int *)((longlong)plVar5 + 0xc) = 0;
        }
        uVar11 = (int)uVar9 + 1;
        uVar9 = (ulonglong)uVar11;
        plVar5 = plVar8;
      } while ((int)uVar11 < (int)*param_3);
    }
    __3_YAXPEAX_Z(param_3);
    goto code_r0x0001806af28d;
  default:
    if (param_3[1] != 0) {
      ___V_YAXPEAX_Z();
      param_3[1] = 0;
      *(int *)param_3 = 0;
    }
    break;
  case 8:
    if (*param_3 != 0) {
      ___V_YAXPEAX_Z();
      *param_3 = 0;
      *(int *)(param_3 + 1) = 0;
    }
    break;
  case 9:
    FUN_1806c1390((ulonglong)*(uint *)(&DAT_1806af2ec + (longlong)(int)param_2 * 4) + 0x180000000,
                  param_3);
    break;
  case 10:
    FUN_180612220(param_3);
  }
  __3_YAXPEAX_Z(param_3);
code_r0x0001806af28d:
  cVar4 = FUN_1805a2010(2);
  if (cVar4 != '\0') {
    uVar6 = FUN_1805a1d70();
    FUN_1805a1f10(uVar6,2,&UNK_180abc4c0,&UNK_180ad10e8,0x4ef3);
  }
  return 0;
}

