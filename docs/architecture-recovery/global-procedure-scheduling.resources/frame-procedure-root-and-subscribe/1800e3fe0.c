
undefined8 FUN_1800e3fe0(longlong param_1,undefined4 param_2)

{
  char cVar1;
  undefined8 *puVar2;
  undefined8 *puVar3;
  undefined8 *puVar4;
  undefined8 *puVar5;
  longlong lVar6;
  undefined4 auStackX_10 [6];
  
  puVar2 = *(undefined8 **)(param_1 + 8);
  puVar3 = (undefined8 *)*puVar2;
  auStackX_10[0] = param_2;
  while( true ) {
    if (puVar3 == puVar2) {
      return 0;
    }
    lVar6 = func_0x00018007c900(puVar3 + 5,auStackX_10);
    if (lVar6 != 0) break;
    if (*(char *)((longlong)puVar3 + 0x19) == '\0') {
      puVar4 = (undefined8 *)puVar3[2];
      if (*(char *)((longlong)puVar4 + 0x19) == '\0') {
        cVar1 = *(char *)((longlong)*puVar4 + 0x19);
        puVar3 = puVar4;
        puVar4 = (undefined8 *)*puVar4;
        while (cVar1 == '\0') {
          cVar1 = *(char *)((longlong)*puVar4 + 0x19);
          puVar3 = puVar4;
          puVar4 = (undefined8 *)*puVar4;
        }
      }
      else {
        cVar1 = *(char *)((longlong)puVar3[1] + 0x19);
        puVar5 = (undefined8 *)puVar3[1];
        puVar4 = puVar3;
        while ((puVar3 = puVar5, cVar1 == '\0' && (puVar4 == (undefined8 *)puVar3[2]))) {
          cVar1 = *(char *)((longlong)puVar3[1] + 0x19);
          puVar5 = (undefined8 *)puVar3[1];
          puVar4 = puVar3;
        }
      }
    }
  }
  return 1;
}

