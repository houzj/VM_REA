
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

int FUN_1800e4080(longlong param_1,int param_2,undefined8 param_3,undefined8 param_4,
                 undefined1 param_5)

{
  char cVar1;
  undefined4 uVar2;
  longlong *plVar3;
  undefined8 *puVar4;
  longlong *plVar5;
  longlong *plVar6;
  longlong *plVar7;
  int iVar8;
  undefined8 *puVar9;
  undefined8 *puVar10;
  undefined8 *puStackX_8;
  int aiStackX_10 [2];
  undefined8 uStackX_18;
  undefined8 uStackX_20;
  longlong *plStack_60;
  longlong lStack_58;
  
  iVar8 = 0;
  plStack_60 = (longlong *)0x0;
  lStack_58 = 0;
  uStackX_18 = param_3;
  uStackX_20 = param_4;
  plStack_60 = (longlong *)FUN_180079000(&plStack_60);
  cVar1 = *(char *)(plStack_60[1] + 0x19);
  plVar6 = (longlong *)plStack_60[1];
  while (cVar1 == '\0') {
    FUN_18003c6d0(&plStack_60,plVar6[2]);
    plVar3 = (longlong *)*plVar6;
    __3_YAXPEAX_Z(plVar6);
    plVar6 = plVar3;
    cVar1 = *(char *)((longlong)plVar3 + 0x19);
  }
  plStack_60[1] = (longlong)plStack_60;
  *plStack_60 = (longlong)plStack_60;
  plStack_60[2] = (longlong)plStack_60;
  lStack_58 = 0;
  puVar4 = *(undefined8 **)(param_1 + 8);
  cVar1 = *(char *)((longlong)puVar4[1] + 0x19);
  puStackX_8 = puVar4;
  puVar10 = (undefined8 *)puVar4[1];
  while (cVar1 == '\0') {
    if (*(int *)(puVar10 + 4) < param_2) {
      puVar9 = (undefined8 *)puVar10[2];
      puVar10 = puStackX_8;
    }
    else {
      puVar9 = (undefined8 *)*puVar10;
    }
    puStackX_8 = puVar10;
    puVar10 = puVar9;
    cVar1 = *(char *)((longlong)puVar9 + 0x19);
  }
  if ((puStackX_8 == puVar4) || (param_2 < *(int *)(puStackX_8 + 4))) {
    puStackX_8 = puVar4;
  }
  if ((puStackX_8 != *(undefined8 **)(param_1 + 8)) &&
     (&plStack_60 != (longlong **)(puStackX_8 + 5))) {
    FUN_180070160(&plStack_60);
    FUN_180077b00(&plStack_60,puStackX_8 + 5,0);
  }
  plVar6 = plStack_60;
  if (lStack_58 == 0) {
    lStack_58 = 0;
    cVar1 = *(char *)(plStack_60[1] + 0x19);
    plVar6 = (longlong *)plStack_60[1];
    while (cVar1 == '\0') {
      FUN_18003c6d0(&plStack_60,plVar6[2]);
      plVar3 = (longlong *)*plVar6;
      __3_YAXPEAX_Z(plVar6);
      plVar6 = plVar3;
      cVar1 = *(char *)((longlong)plVar3 + 0x19);
    }
    plStack_60[1] = (longlong)plStack_60;
  }
  else {
    aiStackX_10[0] = 0;
    plVar3 = (longlong *)*plStack_60;
    while (plVar3 != plVar6) {
      uVar2 = *(undefined4 *)((longlong)plVar3 + 0x1c);
      iVar8 = FUN_1800e4330(param_1,uVar2,param_2,aiStackX_10);
      if ((iVar8 == 0) && (aiStackX_10[0] == 0)) {
        if ((_DAT_1802b2b28 & 1) == 0) {
          _DAT_1802b2b28 = _DAT_1802b2b28 | 1;
          FUN_1801e8930(&UNK_1802029f0);
        }
        FUN_180128200(0x1802b2b2c,uVar2,uStackX_20,0,0,uStackX_18,param_5);
      }
      if (*(char *)((longlong)plVar3 + 0x19) == '\0') {
        plVar5 = (longlong *)plVar3[2];
        if (*(char *)((longlong)plVar5 + 0x19) == '\0') {
          cVar1 = *(char *)(*plVar5 + 0x19);
          plVar3 = plVar5;
          plVar5 = (longlong *)*plVar5;
          while (cVar1 == '\0') {
            cVar1 = *(char *)(*plVar5 + 0x19);
            plVar3 = plVar5;
            plVar5 = (longlong *)*plVar5;
          }
        }
        else {
          cVar1 = *(char *)(plVar3[1] + 0x19);
          plVar7 = (longlong *)plVar3[1];
          plVar5 = plVar3;
          while ((plVar3 = plVar7, cVar1 == '\0' && (plVar5 == (longlong *)plVar3[2]))) {
            cVar1 = *(char *)(plVar3[1] + 0x19);
            plVar7 = (longlong *)plVar3[1];
            plVar5 = plVar3;
          }
        }
      }
    }
    cVar1 = *(char *)(plStack_60[1] + 0x19);
    plVar6 = (longlong *)plStack_60[1];
    while (cVar1 == '\0') {
      FUN_18003c6d0(&plStack_60,plVar6[2]);
      plVar3 = (longlong *)*plVar6;
      __3_YAXPEAX_Z(plVar6);
      plVar6 = plVar3;
      cVar1 = *(char *)((longlong)plVar3 + 0x19);
    }
    plStack_60[1] = (longlong)plStack_60;
  }
  *plStack_60 = (longlong)plStack_60;
  plStack_60[2] = (longlong)plStack_60;
  lStack_58 = 0;
  __3_YAXPEAX_Z(plStack_60);
  return iVar8;
}

