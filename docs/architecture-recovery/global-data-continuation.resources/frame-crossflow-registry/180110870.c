
undefined8 * FUN_180110870(longlong param_1,undefined8 *param_2,int param_3)

{
  int *piVar1;
  char cVar2;
  undefined8 *puVar3;
  undefined8 *puVar4;
  undefined8 uVar5;
  undefined8 *puVar6;
  undefined8 **ppuVar7;
  undefined8 *puVar8;
  undefined8 *apuStackX_8 [3];
  undefined8 *puStackX_20;
  
  puVar3 = *(undefined8 **)(param_1 + 0xa0);
  cVar2 = *(char *)((longlong)puVar3[1] + 0x19);
  puVar4 = puVar3;
  puVar8 = (undefined8 *)puVar3[1];
  while (cVar2 == '\0') {
    if (*(int *)(puVar8 + 4) < param_3) {
      puVar6 = (undefined8 *)puVar8[2];
      puVar8 = puVar4;
    }
    else {
      puVar6 = (undefined8 *)*puVar8;
    }
    puVar4 = puVar8;
    puVar8 = puVar6;
    cVar2 = *(char *)((longlong)puVar6 + 0x19);
  }
  if ((puVar4 == puVar3) || (param_3 < *(int *)(puVar4 + 4))) {
    puStackX_20 = puVar3;
    ppuVar7 = &puStackX_20;
  }
  else {
    apuStackX_8[0] = puVar4;
    ppuVar7 = apuStackX_8;
  }
  puVar4 = *ppuVar7;
  *param_2 = 0;
  param_2[1] = 0;
  if (puVar4 != puVar3) {
    uVar5 = puVar4[5];
    if (puVar4[6] != 0) {
      LOCK();
      piVar1 = (int *)(puVar4[6] + 8);
      *piVar1 = *piVar1 + 1;
      UNLOCK();
    }
    FUN_180015c40(param_2,uVar5);
  }
  return param_2;
}

