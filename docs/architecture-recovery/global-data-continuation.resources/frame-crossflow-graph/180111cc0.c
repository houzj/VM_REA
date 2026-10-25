
undefined8
FUN_180111cc0(longlong param_1,undefined4 param_2,undefined8 param_3,undefined8 param_4,
             undefined8 param_5)

{
  int *piVar1;
  longlong *plVar2;
  longlong *plVar3;
  char cVar4;
  longlong *plVar5;
  undefined8 uVar6;
  undefined1 *puVar7;
  longlong *plVar8;
  undefined4 auStackX_10 [2];
  longlong lStack_68;
  longlong *plStack_60;
  undefined1 auStack_58 [16];
  undefined1 auStack_48 [16];
  undefined1 auStack_38 [16];
  
  auStackX_10[0] = param_2;
  cVar4 = FUN_180110680();
  if (cVar4 == '\0') {
    return 0xe0000322;
  }
  plVar5 = (longlong *)FUN_18009e260(param_1 + 0xd0,auStackX_10);
  plVar8 = *(longlong **)*plVar5;
  if (plVar8 != (longlong *)*plVar5) {
    do {
      FUN_18010a690(param_1,&lStack_68,*(undefined4 *)((longlong)plVar8 + 0x1c));
      plVar2 = plStack_60;
      if (lStack_68 == 0) {
        if (plStack_60 != (longlong *)0x0) {
          LOCK();
          plVar3 = plStack_60 + 1;
          *(int *)plVar3 = (int)*plVar3 + -1;
          UNLOCK();
          if ((int)*plVar3 == 0) {
            (**(code **)*plStack_60)(plStack_60);
            LOCK();
            piVar1 = (int *)((longlong)plVar2 + 0xc);
            *piVar1 = *piVar1 + -1;
            UNLOCK();
            if (*piVar1 == 0) {
              (**(code **)(*plStack_60 + 8))();
            }
          }
        }
        cVar4 = FUN_180110600(param_1,*(undefined4 *)((longlong)plVar8 + 0x1c));
        if (cVar4 != '\0') {
          puVar7 = auStack_58;
          uVar6 = param_4;
          goto LAB_180111dee;
        }
        cVar4 = FUN_180110680(param_1,*(undefined4 *)((longlong)plVar8 + 0x1c));
        if (cVar4 != '\0') {
          puVar7 = auStack_48;
          uVar6 = param_5;
          goto LAB_180111dee;
        }
      }
      else {
        if (plStack_60 != (longlong *)0x0) {
          LOCK();
          plVar3 = plStack_60 + 1;
          *(int *)plVar3 = (int)*plVar3 + -1;
          UNLOCK();
          if ((int)*plVar3 == 0) {
            (**(code **)*plStack_60)(plStack_60);
            LOCK();
            piVar1 = (int *)((longlong)plVar2 + 0xc);
            *piVar1 = *piVar1 + -1;
            UNLOCK();
            if (*piVar1 == 0) {
              (**(code **)(*plStack_60 + 8))();
            }
          }
        }
        puVar7 = auStack_38;
        uVar6 = param_3;
LAB_180111dee:
        FUN_180071a90(uVar6,puVar7,0,(longlong)plVar8 + 0x1c,DAT_1802b1205);
      }
      if (*(char *)((longlong)plVar8 + 0x19) == '\0') {
        plVar2 = (longlong *)plVar8[2];
        if (*(char *)((longlong)plVar2 + 0x19) == '\0') {
          cVar4 = *(char *)(*plVar2 + 0x19);
          plVar8 = plVar2;
          plVar2 = (longlong *)*plVar2;
          while (cVar4 == '\0') {
            cVar4 = *(char *)(*plVar2 + 0x19);
            plVar8 = plVar2;
            plVar2 = (longlong *)*plVar2;
          }
        }
        else {
          cVar4 = *(char *)(plVar8[1] + 0x19);
          plVar3 = (longlong *)plVar8[1];
          plVar2 = plVar8;
          while ((plVar8 = plVar3, cVar4 == '\0' && (plVar2 == (longlong *)plVar8[2]))) {
            cVar4 = *(char *)(plVar8[1] + 0x19);
            plVar3 = (longlong *)plVar8[1];
            plVar2 = plVar8;
          }
        }
      }
    } while (plVar8 != (longlong *)*plVar5);
  }
  return 0;
}

