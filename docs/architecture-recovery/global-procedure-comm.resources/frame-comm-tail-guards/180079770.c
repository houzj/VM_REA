
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180079770(undefined4 *param_1,undefined4 param_2)

{
  int *piVar1;
  char cVar2;
  undefined8 *puVar3;
  undefined8 *puVar4;
  undefined8 *puVar5;
  longlong *plVar6;
  undefined8 uVar7;
  undefined8 *puVar8;
  undefined1 auStack_20 [8];
  longlong *plStack_18;
  
  if ((_DAT_1802b2d60 & 1) == 0) {
    _DAT_1802b2d60 = _DAT_1802b2d60 | 1;
    FUN_18007c9e0();
    FUN_1801e8930(&UNK_180202be0);
  }
  plVar6 = (longlong *)FUN_18007d5e0(&DAT_1802b2d68,*param_1);
  if (plVar6[1] != 0) {
    uVar7 = FUN_18008e6c0(*(undefined8 *)(param_1 + 2));
    FUN_1800cb9e0(uVar7,param_1[1]);
    puVar8 = (undefined8 *)*plVar6;
    puVar3 = (undefined8 *)*puVar8;
    while (puVar3 != puVar8) {
      switch(*(undefined4 *)(puVar3 + 8)) {
      case 1:
        FUN_180079960(param_1);
        break;
      case 2:
        FUN_180079b00(param_1);
        break;
      case 3:
        FUN_180079e40(param_1);
        break;
      case 4:
        FUN_180079fb0(param_1);
        break;
      case 5:
        FUN_18007a050(param_1);
        break;
      case 6:
        FUN_18007a0f0(param_1);
        break;
      case 7:
        FUN_180079ca0(param_1);
      }
      if (*(char *)((longlong)puVar3 + 0x19) == '\0') {
        puVar4 = (undefined8 *)puVar3[2];
        if (*(char *)((longlong)puVar4 + 0x19) == '\0') {
          cVar2 = *(char *)((longlong)*puVar4 + 0x19);
          puVar3 = puVar4;
          puVar4 = (undefined8 *)*puVar4;
          while (cVar2 == '\0') {
            cVar2 = *(char *)((longlong)*puVar4 + 0x19);
            puVar3 = puVar4;
            puVar4 = (undefined8 *)*puVar4;
          }
        }
        else {
          cVar2 = *(char *)((longlong)puVar3[1] + 0x19);
          puVar5 = (undefined8 *)puVar3[1];
          puVar4 = puVar3;
          while ((puVar3 = puVar5, cVar2 == '\0' && (puVar4 == (undefined8 *)puVar3[2]))) {
            cVar2 = *(char *)((longlong)puVar3[1] + 0x19);
            puVar5 = (undefined8 *)puVar3[1];
            puVar4 = puVar3;
          }
        }
      }
    }
    uVar7 = FUN_18008e6c0(*(undefined8 *)(param_1 + 2));
    FUN_1800ccfc0(uVar7,*param_1,param_1[1]);
    puVar8 = (undefined8 *)FUN_1800d33b0(auStack_20);
    FUN_1800d34a0(*puVar8,*param_1,param_1[1],*(undefined8 *)(param_1 + 2),param_2);
    if (plStack_18 != (longlong *)0x0) {
      LOCK();
      plVar6 = plStack_18 + 1;
      *(int *)plVar6 = (int)*plVar6 + -1;
      UNLOCK();
      if ((int)*plVar6 == 0) {
        (**(code **)*plStack_18)(plStack_18);
        LOCK();
        piVar1 = (int *)((longlong)plStack_18 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (*piVar1 == 0) {
          (**(code **)(*plStack_18 + 8))(plStack_18);
        }
      }
    }
  }
  return;
}

