
undefined8 FUN_1800de620(longlong param_1)

{
  longlong *plVar1;
  int *piVar2;
  longlong *plVar3;
  undefined4 uVar4;
  undefined8 uVar5;
  undefined4 *puVar6;
  undefined8 uStack_20;
  longlong *plStack_18;
  
  if (*(longlong *)(param_1 + 0x80) == 0) {
    uVar5 = CreateEventW(0,1,1,0);
    *(undefined8 *)(param_1 + 0x80) = uVar5;
  }
  if (*(longlong *)(param_1 + 0x88) == 0) {
    uVar5 = CreateEventW(0,1,1,0);
    *(undefined8 *)(param_1 + 0x88) = uVar5;
  }
  if ((*(longlong *)(param_1 + 0x80) == 0) || (*(longlong *)(param_1 + 0x88) == 0)) {
    uVar5 = 0xe0000002;
  }
  else {
    if (*(longlong *)(param_1 + 0xf0) == 0) {
      puVar6 = (undefined4 *)__2_YAPEAX_K_Z(0x10);
      if (puVar6 == (undefined4 *)0x0) {
        puVar6 = (undefined4 *)0x0;
      }
      else {
        uVar5 = *(undefined8 *)(param_1 + 0x198);
        *puVar6 = *(undefined4 *)(param_1 + 0x50);
        *(undefined8 *)(puVar6 + 2) = uVar5;
        uVar4 = FUN_18007c6d0();
        puVar6[1] = uVar4;
        uVar5 = FUN_18008e6c0(uVar5);
        FUN_1800cb220(uVar5,puVar6[1]);
      }
      uStack_20 = 0;
      plStack_18 = (longlong *)0x0;
      FUN_1800e2ad0(&uStack_20,puVar6);
      FUN_18001d870(param_1 + 0xf0,&uStack_20);
      plVar3 = plStack_18;
      if (plStack_18 != (longlong *)0x0) {
        LOCK();
        plVar1 = plStack_18 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plStack_18)(plStack_18);
          LOCK();
          piVar2 = (int *)((longlong)plVar3 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plStack_18 + 8))();
          }
        }
      }
    }
    uVar5 = 0;
  }
  return uVar5;
}

