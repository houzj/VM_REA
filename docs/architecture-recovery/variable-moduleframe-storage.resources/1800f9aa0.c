
longlong FUN_1800f9aa0(undefined8 param_1,undefined8 param_2)

{
  char cVar1;
  longlong *plVar2;
  longlong *plVar3;
  longlong lVar4;
  undefined1 auStackX_18 [16];
  longlong *plStack_18;
  longlong *plStack_10;
  
  FUN_18007e660(param_1,&plStack_18,param_2);
  lVar4 = 0;
  while (plStack_18 != plStack_10) {
    lVar4 = lVar4 + 1;
    if (*(char *)((longlong)plStack_18 + 0x19) == '\0') {
      plVar2 = (longlong *)plStack_18[2];
      if (*(char *)((longlong)plVar2 + 0x19) == '\0') {
        cVar1 = *(char *)(*plVar2 + 0x19);
        plStack_18 = plVar2;
        plVar2 = (longlong *)*plVar2;
        while (cVar1 == '\0') {
          cVar1 = *(char *)(*plVar2 + 0x19);
          plStack_18 = plVar2;
          plVar2 = (longlong *)*plVar2;
        }
      }
      else {
        cVar1 = *(char *)(plStack_18[1] + 0x19);
        plVar3 = (longlong *)plStack_18[1];
        plVar2 = plStack_18;
        while ((plStack_18 = plVar3, cVar1 == '\0' && (plVar2 == (longlong *)plStack_18[2]))) {
          cVar1 = *(char *)(plStack_18[1] + 0x19);
          plVar3 = (longlong *)plStack_18[1];
          plVar2 = plStack_18;
        }
      }
    }
  }
  FUN_18006eb00(param_1,auStackX_18);
  return lVar4;
}

