
undefined8 *
FUN_18006eb00(longlong *param_1,undefined8 *param_2,longlong *param_3,longlong *param_4)

{
  char cVar1;
  longlong *plVar2;
  longlong *plVar3;
  undefined1 auStackX_8 [8];
  longlong *plStackX_18;
  
  plVar2 = (longlong *)*param_1;
  if ((param_3 == (longlong *)*plVar2) && (param_4 == plVar2)) {
    FUN_18006e570(param_1,plVar2[1]);
    *(longlong *)(*param_1 + 8) = *param_1;
    *(longlong *)*param_1 = *param_1;
    *(longlong *)(*param_1 + 0x10) = *param_1;
    param_1[1] = 0;
    *param_2 = *(undefined8 *)*param_1;
    return param_2;
  }
  while (param_3 != param_4) {
    plStackX_18 = param_3;
    if (*(char *)((longlong)param_3 + 0x19) == '\0') {
      plStackX_18 = (longlong *)param_3[2];
      if (*(char *)((longlong)plStackX_18 + 0x19) == '\0') {
        cVar1 = *(char *)(*plStackX_18 + 0x19);
        plVar2 = (longlong *)*plStackX_18;
        while (cVar1 == '\0') {
          cVar1 = *(char *)(*plVar2 + 0x19);
          plStackX_18 = plVar2;
          plVar2 = (longlong *)*plVar2;
        }
      }
      else {
        cVar1 = *(char *)(param_3[1] + 0x19);
        plVar3 = (longlong *)param_3[1];
        plVar2 = param_3;
        while ((plStackX_18 = plVar3, cVar1 == '\0' && (plVar2 == (longlong *)plStackX_18[2]))) {
          cVar1 = *(char *)(plStackX_18[1] + 0x19);
          plVar3 = (longlong *)plStackX_18[1];
          plVar2 = plStackX_18;
        }
      }
    }
    FUN_18006f290(param_1,auStackX_8,param_3);
    param_3 = plStackX_18;
  }
  *param_2 = param_3;
  return param_2;
}

