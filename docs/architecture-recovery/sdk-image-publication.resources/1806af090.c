
undefined8 FUN_1806af090(undefined8 param_1,uint param_2,longlong *param_3)

{
  longlong *plVar1;
  char cVar2;
  undefined8 uVar3;
  ulonglong uVar4;
  longlong *plVar5;
  ulonglong uVar6;
  ulonglong uVar7;
  uint uVar8;
  ulonglong uVar9;
  
  cVar2 = FUN_1805a2010(2);
  if (cVar2 != '\0') {
    uVar3 = FUN_1805a1d70();
    FUN_1805a1f10(uVar3,2,&UNK_180abc4d0,&UNK_180ad10e8,0x4e59);
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
      uVar4 = 0;
      uVar6 = uVar4;
      if (0 < (int)*param_3) {
        do {
          if (*(longlong *)(uVar4 + 0x80 + param_3[1]) != 0) {
            ___V_YAXPEAX_Z();
            *(undefined8 *)(uVar4 + 0x80 + param_3[1]) = 0;
          }
          *(undefined4 *)(uVar4 + 0x88 + param_3[1]) = 0;
          memset((longlong)(int)uVar6 * 0x90 + param_3[1],0,0x80);
          uVar8 = (int)uVar6 + 1;
          uVar4 = uVar4 + 0x90;
          uVar6 = (ulonglong)uVar8;
        } while ((int)uVar8 < (int)*param_3);
      }
      *(int *)param_3 = 0;
      ___V_YAXPEAX_Z(param_3[1]);
      param_3[1] = 0;
    }
    break;
  case 4:
    uVar4 = 0;
    uVar6 = uVar4;
    plVar1 = param_3;
    if (0 < (int)*param_3) {
      do {
        plVar5 = plVar1 + 2;
        if (*plVar5 != 0) {
          uVar7 = uVar4;
          uVar9 = uVar4;
          if (0 < *(int *)((longlong)plVar1 + 0xc)) {
            do {
              if (*(longlong *)(uVar7 + 0x80 + *plVar5) != 0) {
                ___V_YAXPEAX_Z();
                *(undefined8 *)(uVar7 + 0x80 + *plVar5) = 0;
              }
              uVar8 = (int)uVar9 + 1;
              uVar7 = uVar7 + 0x90;
              uVar9 = (ulonglong)uVar8;
            } while ((int)uVar8 < *(int *)((longlong)plVar1 + 0xc));
          }
          ___V_YAXPEAX_Z(*plVar5);
          *plVar5 = 0;
          *(int *)((longlong)plVar1 + 0xc) = 0;
        }
        uVar8 = (int)uVar6 + 1;
        uVar6 = (ulonglong)uVar8;
        plVar1 = plVar5;
      } while ((int)uVar8 < (int)*param_3);
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
  cVar2 = FUN_1805a2010(2);
  if (cVar2 != '\0') {
    uVar3 = FUN_1805a1d70();
    FUN_1805a1f10(uVar3,2,&UNK_180abc4c0,&UNK_180ad10e8,0x4ef3);
  }
  return 0;
}

