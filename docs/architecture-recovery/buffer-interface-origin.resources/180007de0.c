
void FUN_180007de0(longlong *param_1)

{
  longlong *plVar1;
  int *piVar2;
  char cVar3;
  undefined8 *puVar4;
  undefined8 *puVar5;
  undefined8 *puVar6;
  undefined8 *puVar7;
  longlong *plVar8;
  longlong *plVar9;
  longlong *plVar10;
  undefined8 local_res10;
  
  *param_1 = (longlong)CModuleSharedMemoryBase::vftable;
  puVar4 = (undefined8 *)param_1[0xe];
  puVar5 = (undefined8 *)*puVar4;
  plVar10 = param_1;
  while (puVar5 != puVar4) {
    puVar6 = (undefined8 *)puVar5[5];
    for (puVar7 = (undefined8 *)*puVar6; puVar7 != puVar6; puVar7 = (undefined8 *)*puVar7) {
      FUN_1800090f0(plVar10,0xffffffff,(undefined8 *)puVar7[2]);
    }
    plVar8 = (longlong *)puVar5[5];
    plVar9 = (longlong *)*plVar8;
    *plVar8 = (longlong)plVar8;
    *(undefined8 *)(puVar5[5] + 8) = puVar5[5];
    puVar5[6] = 0;
    if (plVar9 != (longlong *)puVar5[5]) {
      do {
        plVar10 = plVar9;
        plVar9 = (longlong *)*plVar10;
        plVar8 = (longlong *)plVar10[3];
        if (plVar8 != (longlong *)0x0) {
          LOCK();
          plVar1 = plVar8 + 1;
          *(int *)plVar1 = (int)*plVar1 + -1;
          UNLOCK();
          if ((int)*plVar1 == 0) {
            (**(code **)*plVar8)(plVar8);
            LOCK();
            piVar2 = (int *)((longlong)plVar8 + 0xc);
            *piVar2 = *piVar2 + -1;
            UNLOCK();
            if (*piVar2 == 0) {
              (**(code **)(*plVar8 + 8))(plVar8);
            }
          }
        }
        operator_delete(plVar10);
      } while (plVar9 != (longlong *)puVar5[5]);
    }
    if (*(char *)((longlong)puVar5 + 0x19) == '\0') {
      puVar6 = (undefined8 *)puVar5[2];
      if (*(char *)((longlong)puVar6 + 0x19) == '\0') {
        cVar3 = *(char *)((longlong)*puVar6 + 0x19);
        puVar5 = puVar6;
        puVar6 = (undefined8 *)*puVar6;
        while (cVar3 == '\0') {
          cVar3 = *(char *)((longlong)*puVar6 + 0x19);
          puVar5 = puVar6;
          puVar6 = (undefined8 *)*puVar6;
        }
      }
      else {
        cVar3 = *(char *)((longlong)puVar5[1] + 0x19);
        puVar7 = (undefined8 *)puVar5[1];
        puVar6 = puVar5;
        while ((puVar5 = puVar7, cVar3 == '\0' && (puVar6 == (undefined8 *)puVar5[2]))) {
          cVar3 = *(char *)((longlong)puVar5[1] + 0x19);
          puVar7 = (undefined8 *)puVar5[1];
          puVar6 = puVar5;
        }
      }
    }
  }
  FUN_180009a40(param_1 + 0xe,&local_res10,*(longlong **)param_1[0xe],(longlong *)param_1[0xe]);
  operator_delete((void *)param_1[0xe]);
  cVar3 = *(char *)((longlong)*(longlong **)(param_1[0xc] + 8) + 0x19);
  plVar10 = *(longlong **)(param_1[0xc] + 8);
  while (cVar3 == '\0') {
    FUN_18000a4e0(param_1 + 0xc,(longlong *)plVar10[2]);
    plVar8 = (longlong *)*plVar10;
    operator_delete(plVar10);
    plVar10 = plVar8;
    cVar3 = *(char *)((longlong)plVar8 + 0x19);
  }
  *(longlong *)(param_1[0xc] + 8) = param_1[0xc];
  *(longlong *)param_1[0xc] = param_1[0xc];
  *(longlong *)(param_1[0xc] + 0x10) = param_1[0xc];
  param_1[0xd] = 0;
                    /* WARNING: Could not recover jumptable at 0x000180007f98. Too many branches */
                    /* WARNING: Treating indirect jump as call */
  operator_delete((void *)param_1[0xc]);
  return;
}

