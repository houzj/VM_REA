
int * FUN_1800e4570(longlong *param_1,int *param_2)

{
  undefined8 *puVar1;
  undefined8 *puVar2;
  undefined8 *puVar3;
  longlong lVar4;
  undefined8 *puVar5;
  int *piStackX_8;
  
  puVar1 = (undefined8 *)*param_1;
  puVar5 = puVar1;
  if (*(char *)((longlong)puVar1[1] + 0x19) == '\0') {
    puVar2 = (undefined8 *)puVar1[1];
    do {
      if (*(int *)(puVar2 + 4) < *param_2) {
        puVar3 = (undefined8 *)puVar2[2];
      }
      else {
        puVar3 = (undefined8 *)*puVar2;
        puVar5 = puVar2;
      }
      puVar2 = puVar3;
    } while (*(char *)((longlong)puVar3 + 0x19) == '\0');
  }
  if ((puVar5 != puVar1) && (*(int *)(puVar5 + 4) <= *param_2)) {
    return (int *)(puVar5 + 5);
  }
  piStackX_8 = param_2;
  lVar4 = FUN_1800e4da0(param_1,param_2,&piStackX_8);
  FUN_1800e4e40(param_1,&piStackX_8,puVar5,lVar4 + 0x20,lVar4);
  return piStackX_8 + 10;
}

