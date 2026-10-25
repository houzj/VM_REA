
undefined8 FUN_18009a140(longlong param_1,undefined4 param_2,undefined4 param_3,byte param_4)

{
  longlong *plVar1;
  char cVar2;
  int iVar3;
  char *pcVar4;
  undefined8 uVar5;
  undefined8 uVar6;
  undefined *puVar7;
  char *pcStackX_8;
  undefined4 auStackX_18 [4];
  undefined4 uVar8;
  
  cVar2 = *(char *)(param_1 + 0x10);
  if ((param_4 & cVar2 == '\0') != 0) {
    return 0;
  }
  plVar1 = (longlong *)(param_1 + 0x18);
  auStackX_18[0] = param_3;
  FUN_18009ab20(plVar1,&pcStackX_8,auStackX_18);
  if (pcStackX_8 == (char *)*plVar1) {
    cVar2 = FUN_18001dbc0(4);
    if (cVar2 == '\0') {
      return 1;
    }
    uVar5 = FUN_18001db50();
    puVar7 = &DAT_180226c50;
    uVar6 = 4;
    uVar8 = 0x5d6;
  }
  else if (param_4 == 0) {
    pcVar4 = (char *)FUN_18009a990(plVar1,auStackX_18);
    if (pcVar4[1] == '\0') {
      return 1;
    }
    if (*pcVar4 != '\0') {
      FUN_18009a300();
    }
    FUN_1800069c0(*(longlong *)(pcVar4 + 0x18) + 0x38);
    pcVar4[1] = '\0';
    iVar3 = _Mtx_unlock(*(undefined8 *)(pcVar4 + 8));
    if (iVar3 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar3);
    }
    cVar2 = FUN_18001dbc0(1);
    if (cVar2 == '\0') {
      return 1;
    }
    uVar5 = FUN_18001db50();
    puVar7 = &DAT_180226c78;
    uVar6 = 1;
    uVar8 = 0x5d0;
  }
  else {
    if (cVar2 == '\0') {
      return 0;
    }
    pcVar4 = (char *)FUN_18009a990(plVar1,auStackX_18);
    if (*pcVar4 == '\0') {
      return 0;
    }
    iVar3 = _Mtx_lock(*(undefined8 *)(pcVar4 + 8));
    if (iVar3 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar3);
    }
    pcVar4[1] = '\x01';
    cVar2 = FUN_18001dbc0(1);
    if (cVar2 == '\0') {
      return 1;
    }
    uVar5 = FUN_18001db50();
    puVar7 = &DAT_180226bf0;
    uVar6 = 1;
    uVar8 = 0x5b7;
  }
  FUN_180015b40(uVar5,uVar6,puVar7,&DAT_180226c18,uVar8,param_2,auStackX_18[0]);
  return 1;
}

