
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

int * FUN_18009df90(longlong param_1,undefined8 param_2)

{
  longlong *plVar1;
  char cVar2;
  int *piVar3;
  int iVar4;
  int *piVar5;
  longlong lVar6;
  int *piVar7;
  int *piVar8;
  int *piStackX_8;
  int aiStackX_10 [2];
  
  aiStackX_10[0] = (int)param_2;
  plVar1 = (longlong *)(param_1 + 0x30);
  piVar3 = (int *)*plVar1;
  cVar2 = *(char *)((longlong)*(int **)(piVar3 + 2) + 0x19);
  piStackX_8 = piVar3;
  piVar8 = *(int **)(piVar3 + 2);
  while (cVar2 == '\0') {
    if (piVar8[8] < aiStackX_10[0]) {
      piVar7 = *(int **)(piVar8 + 4);
      piVar8 = piStackX_8;
    }
    else {
      piVar7 = *(int **)piVar8;
    }
    piStackX_8 = piVar8;
    piVar8 = piVar7;
    cVar2 = *(char *)((longlong)piVar7 + 0x19);
  }
  if ((piStackX_8 == piVar3) || (aiStackX_10[0] < piStackX_8[8])) {
    piStackX_8 = piVar3;
  }
  if (piStackX_8 == piVar3) {
    piVar3 = (int *)(param_1 + 0x28);
    piStackX_8 = piVar3;
    iVar4 = _Mtx_lock(piVar3);
    if (iVar4 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar4);
    }
    if ((_DAT_1802b35c0 & 1) == 0) {
      _DAT_1802b35c0 = _DAT_1802b35c0 | 1;
      _DAT_1802b35d0 = 0;
      _DAT_1802b35d8 = 0;
      _DAT_1802b35d0 = FUN_18009f110();
      FUN_1801e8930(FUN_180203260);
    }
    iVar4 = _Mtx_unlock(piVar3);
    if (iVar4 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar4);
    }
    piStackX_8 = (int *)&DAT_1802b35d0;
  }
  else {
    cVar2 = *(char *)((longlong)*(int **)(piVar3 + 2) + 0x19);
    piVar8 = piVar3;
    piVar7 = *(int **)(piVar3 + 2);
    while (cVar2 == '\0') {
      if (piVar7[8] < aiStackX_10[0]) {
        piVar5 = *(int **)(piVar7 + 4);
        piVar7 = piVar8;
      }
      else {
        piVar5 = *(int **)piVar7;
      }
      piVar8 = piVar7;
      piVar7 = piVar5;
      cVar2 = *(char *)((longlong)piVar5 + 0x19);
    }
    if ((piVar8 == piVar3) || (piStackX_8 = piVar8, aiStackX_10[0] < piVar8[8])) {
      piStackX_8 = aiStackX_10;
      lVar6 = FUN_18009f2f0(plVar1,param_2,&piStackX_8);
      FUN_18009f370(plVar1,&piStackX_8,piVar8,lVar6 + 0x20,lVar6);
    }
    piStackX_8 = piStackX_8 + 10;
  }
  return piStackX_8;
}

