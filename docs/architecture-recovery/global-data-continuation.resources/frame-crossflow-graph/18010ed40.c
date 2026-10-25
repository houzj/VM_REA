
undefined8 FUN_18010ed40(undefined8 param_1,undefined8 param_2,undefined8 param_3,longlong *param_4)

{
  char cVar1;
  longlong *plVar2;
  longlong *plVar3;
  longlong *plVar4;
  longlong *plVar5;
  longlong *plStack_38;
  undefined8 uStack_30;
  
  FUN_180076af0(param_2,param_3,param_3,param_4,0,0xfffffffffffffffe);
  plVar5 = *(longlong **)*param_4;
  if (plVar5 != (longlong *)*param_4) {
    do {
      plStack_38 = (longlong *)0x0;
      uStack_30 = 0;
      plStack_38 = (longlong *)FUN_180079000(&plStack_38);
      FUN_180111880(param_1,*(undefined4 *)((longlong)plVar5 + 0x1c),&plStack_38);
      plVar4 = (longlong *)*plStack_38;
      if (plVar4 != plStack_38) {
        do {
          FUN_18006ddb0(param_2,(longlong)plVar4 + 0x1c);
          if (*(char *)((longlong)plVar4 + 0x19) == '\0') {
            plVar2 = (longlong *)plVar4[2];
            if (*(char *)((longlong)plVar2 + 0x19) == '\0') {
              cVar1 = *(char *)(*plVar2 + 0x19);
              plVar4 = plVar2;
              plVar2 = (longlong *)*plVar2;
              while (cVar1 == '\0') {
                cVar1 = *(char *)(*plVar2 + 0x19);
                plVar4 = plVar2;
                plVar2 = (longlong *)*plVar2;
              }
            }
            else {
              cVar1 = *(char *)(plVar4[1] + 0x19);
              plVar3 = (longlong *)plVar4[1];
              plVar2 = plVar4;
              while ((plVar4 = plVar3, cVar1 == '\0' && (plVar2 == (longlong *)plVar4[2]))) {
                cVar1 = *(char *)(plVar4[1] + 0x19);
                plVar3 = (longlong *)plVar4[1];
                plVar2 = plVar4;
              }
            }
          }
        } while (plVar4 != plStack_38);
      }
      cVar1 = *(char *)(plStack_38[1] + 0x19);
      plVar4 = (longlong *)plStack_38[1];
      while (cVar1 == '\0') {
        FUN_18003c6d0(&plStack_38,plVar4[2]);
        plVar2 = (longlong *)*plVar4;
        __3_YAXPEAX_Z(plVar4);
        plVar4 = plVar2;
        cVar1 = *(char *)((longlong)plVar2 + 0x19);
      }
      plStack_38[1] = (longlong)plStack_38;
      *plStack_38 = (longlong)plStack_38;
      plStack_38[2] = (longlong)plStack_38;
      uStack_30 = 0;
      __3_YAXPEAX_Z(plStack_38);
      if (*(char *)((longlong)plVar5 + 0x19) == '\0') {
        plVar4 = (longlong *)plVar5[2];
        if (*(char *)((longlong)plVar4 + 0x19) == '\0') {
          cVar1 = *(char *)(*plVar4 + 0x19);
          plVar5 = plVar4;
          plVar4 = (longlong *)*plVar4;
          while (cVar1 == '\0') {
            cVar1 = *(char *)(*plVar4 + 0x19);
            plVar5 = plVar4;
            plVar4 = (longlong *)*plVar4;
          }
        }
        else {
          cVar1 = *(char *)(plVar5[1] + 0x19);
          plVar2 = (longlong *)plVar5[1];
          plVar4 = plVar5;
          while ((plVar5 = plVar2, cVar1 == '\0' && (plVar4 == (longlong *)plVar5[2]))) {
            cVar1 = *(char *)(plVar5[1] + 0x19);
            plVar2 = (longlong *)plVar5[1];
            plVar4 = plVar5;
          }
        }
      }
    } while (plVar5 != (longlong *)*param_4);
  }
  return param_2;
}

