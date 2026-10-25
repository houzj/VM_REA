
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

int FUN_180014140(longlong param_1,undefined8 param_2,undefined8 param_3,int *param_4)

{
  char cVar1;
  int iVar2;
  undefined8 *puVar3;
  longlong lVar4;
  undefined *puVar5;
  int aiStackX_20 [2];
  undefined4 *puVar6;
  undefined8 uVar7;
  undefined8 in_stack_ffffffffffffffa0;
  undefined4 uVar9;
  int *piVar8;
  undefined8 in_stack_ffffffffffffffb0;
  undefined4 uVar10;
  undefined4 uStack_48;
  int iStack_44;
  float afStack_40 [2];
  undefined8 uStack_38;
  
  uVar9 = (undefined4)((ulonglong)in_stack_ffffffffffffffa0 >> 0x20);
  uVar10 = (undefined4)((ulonglong)in_stack_ffffffffffffffb0 >> 0x20);
  uStack_38 = 0xfffffffffffffffe;
  aiStackX_20[0] = 0;
  iStack_44 = 0;
  afStack_40[0] = 0.0;
  if ((*param_4 == 3) && (*(longlong *)(param_4 + 2) != 0)) {
    ___V_YAXPEAX_Z();
    param_4[2] = 0;
    param_4[3] = 0;
    param_4[1] = 0;
  }
  uStack_48 = 0;
  iVar2 = VM_M_GetInt(param_2,param_3,0,&iStack_44,&uStack_48);
  if (iVar2 == 0) {
    cVar1 = HSlog_ShouldLog(2);
    if (cVar1 != '\0') {
      puVar3 = (undefined8 *)FUN_1800013b0();
      if (0xf < (ulonglong)puVar3[3]) {
        puVar3 = (undefined8 *)*puVar3;
      }
      HSlog_LogPrint(2,&UNK_18001d078,puVar3,*(undefined4 *)(param_1 + 0x48),&UNK_18001d058,
                     CONCAT44(uVar9,0x1a4),param_3,CONCAT44(uVar10,iStack_44));
    }
    *param_4 = 1;
    param_4[2] = iStack_44;
    return 0;
  }
  puVar6 = &uStack_48;
  iVar2 = VM_M_GetFloat(param_2,param_3,0,afStack_40,puVar6);
  uVar10 = (undefined4)((ulonglong)puVar6 >> 0x20);
  if (iVar2 == 0) {
    cVar1 = HSlog_ShouldLog(2);
    if (cVar1 != '\0') {
      puVar3 = (undefined8 *)FUN_1800013b0();
      if (0xf < (ulonglong)puVar3[3]) {
        puVar3 = (undefined8 *)*puVar3;
      }
      HSlog_LogPrint(2,&UNK_18001d0a0,puVar3,*(undefined4 *)(param_1 + 0x48),&UNK_18001d058,
                     CONCAT44(uVar9,0x1ad),param_3,(double)afStack_40[0]);
    }
    *param_4 = 2;
    param_4[2] = (int)afStack_40[0];
    return 0;
  }
  aiStackX_20[0] = 0x100;
  lVar4 = func_0x000180018cf0(0x100,0x1800258e0);
  if (lVar4 == 0) {
LAB_1800143f7:
    iVar2 = -0x1ffffffd;
  }
  else {
    piVar8 = aiStackX_20;
    uVar7 = CONCAT44(uVar10,aiStackX_20[0]);
    iVar2 = VM_M_GetString(param_2,param_3,0,lVar4,uVar7,piVar8,&uStack_48);
    uVar9 = (undefined4)((ulonglong)piVar8 >> 0x20);
    uVar10 = (undefined4)((ulonglong)uVar7 >> 0x20);
    if (iVar2 == -0x1fffffee) {
      ___V_YAXPEAX_Z(lVar4);
      lVar4 = func_0x000180018cf0((longlong)aiStackX_20[0],0x1800258e0);
      if (lVar4 == 0) goto LAB_1800143f7;
      piVar8 = aiStackX_20;
      iVar2 = VM_M_GetString(param_2,param_3,0,lVar4,CONCAT44(uVar10,aiStackX_20[0]),piVar8,
                             &uStack_48);
      uVar9 = (undefined4)((ulonglong)piVar8 >> 0x20);
    }
    if (iVar2 == 0) {
      cVar1 = HSlog_ShouldLog(2);
      if (cVar1 != '\0') {
        puVar3 = (undefined8 *)FUN_1800013b0();
        if (0xf < (ulonglong)puVar3[3]) {
          puVar3 = (undefined8 *)*puVar3;
        }
        HSlog_LogPrint(2,&UNK_18001d0d0,puVar3,*(undefined4 *)(param_1 + 0x48),&UNK_18001d058,
                       CONCAT44(uVar9,0x1cd),param_3,lVar4);
      }
      *param_4 = 3;
      param_4[1] = aiStackX_20[0];
      *(longlong *)(param_4 + 2) = lVar4;
      return 0;
    }
  }
  *param_4 = 0;
  cVar1 = HSlog_ShouldLog(4);
  if (cVar1 != '\0') {
    if ((_DAT_180025eb0 & 1) == 0) {
      _DAT_180025eb0 = _DAT_180025eb0 | 1;
      FUN_1800013f0();
      FUN_180018df8(FUN_18001a860);
    }
    puVar5 = &DAT_180025e90;
    if (0xf < _DAT_180025ea8) {
      puVar5 = _DAT_180025e90;
    }
    HSlog_LogPrint(4,&UNK_18001d0f8,puVar5,*(undefined4 *)(param_1 + 0x48),&UNK_18001d058,
                   CONCAT44(uVar9,0x1e4),param_3);
  }
  if (lVar4 != 0) {
    ___V_YAXPEAX_Z(lVar4);
  }
  return iVar2;
}

