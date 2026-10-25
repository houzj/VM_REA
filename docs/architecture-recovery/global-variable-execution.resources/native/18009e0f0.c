
int * FUN_18009e0f0(longlong param_1,undefined8 param_2)

{
  char cVar1;
  int *piVar2;
  int *piVar3;
  longlong lVar4;
  int *piVar5;
  int *piVar6;
  int *piStackX_8;
  int aiStackX_10 [2];
  
  aiStackX_10[0] = (int)param_2;
  piVar2 = *(int **)(param_1 + 0x40);
  cVar1 = *(char *)((longlong)*(int **)(piVar2 + 2) + 0x19);
  piStackX_8 = piVar2;
  piVar6 = *(int **)(piVar2 + 2);
  while (cVar1 == '\0') {
    if (piVar6[8] < aiStackX_10[0]) {
      piVar5 = *(int **)(piVar6 + 4);
      piVar6 = piStackX_8;
    }
    else {
      piVar5 = *(int **)piVar6;
    }
    piStackX_8 = piVar6;
    piVar6 = piVar5;
    cVar1 = *(char *)((longlong)piVar5 + 0x19);
  }
  if ((piStackX_8 == piVar2) || (aiStackX_10[0] < piStackX_8[8])) {
    piStackX_8 = piVar2;
  }
  if (piStackX_8 == piVar2) {
    return (int *)(param_1 + 0x18);
  }
  cVar1 = *(char *)((longlong)*(int **)(piVar2 + 2) + 0x19);
  piVar6 = piVar2;
  piVar5 = *(int **)(piVar2 + 2);
  while (cVar1 == '\0') {
    if (piVar5[8] < aiStackX_10[0]) {
      piVar3 = *(int **)(piVar5 + 4);
      piVar5 = piVar6;
    }
    else {
      piVar3 = *(int **)piVar5;
    }
    piVar6 = piVar5;
    piVar5 = piVar3;
    cVar1 = *(char *)((longlong)piVar3 + 0x19);
  }
  if ((piVar6 == piVar2) || (piStackX_8 = piVar6, aiStackX_10[0] < piVar6[8])) {
    piStackX_8 = aiStackX_10;
    lVar4 = FUN_18009f5c0(param_1 + 0x40,param_2,&piStackX_8);
    FUN_18009f640(param_1 + 0x40,&piStackX_8,piVar6,lVar4 + 0x20,lVar4);
  }
  return piStackX_8 + 10;
}

