
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800eff60(undefined8 *param_1,undefined8 *param_2,longlong param_3)

{
  char cVar1;
  undefined4 uVar2;
  undefined8 uVar3;
  undefined8 *puVar4;
  undefined *puVar5;
  undefined1 auStack_88 [32];
  undefined8 uStack_68;
  undefined8 *puStack_60;
  undefined8 uStack_58;
  undefined4 uStack_50;
  undefined8 *puStack_48;
  undefined8 uStack_40;
  undefined8 *puStack_38;
  ulonglong uStack_30;
  
  uStack_40 = 0xfffffffffffffffe;
  uStack_30 = _DAT_1802a5230 ^ (ulonglong)auStack_88;
  param_1[3] = 0xf;
  param_1[2] = 0;
  *(undefined1 *)param_1 = 0;
  param_1[6] = 0;
  puStack_48 = param_1;
  puStack_38 = param_2;
  if (param_1 != param_2) {
    FUN_180006470(param_1,param_2,0,0xffffffffffffffff);
  }
  param_1[5] = param_3 + 0x80;
  puVar4 = param_2;
  if (0xf < (ulonglong)param_2[3]) {
    puVar4 = (undefined8 *)*param_2;
  }
  uVar3 = OpenFileMappingA(0xf001f,0,puVar4);
  uVar2 = GetLastError();
  param_1[4] = uVar3;
  cVar1 = FUN_18001dbc0(0);
  if (cVar1 != '\0') {
    puVar4 = param_2;
    if (0xf < (ulonglong)param_2[3]) {
      puVar4 = (undefined8 *)*param_2;
    }
    uVar3 = FUN_18001db50();
    uStack_58 = CONCAT44(uStack_58._4_4_,uVar2);
    uStack_68 = CONCAT44(uStack_68._4_4_,0x145);
    puStack_60 = puVar4;
    FUN_180015b40(uVar3,0,&UNK_18022e268,&UNK_18022e1d8);
  }
  if (param_1[4] == 0) {
    cVar1 = FUN_18001dbc0(4);
    if (cVar1 != '\0') {
      uVar3 = FUN_18001db50();
      uStack_68 = CONCAT44(uStack_68._4_4_,0x153);
      puVar5 = &UNK_18022e290;
LAB_1800f0117:
      FUN_180015b40(uVar3,4,puVar5,&UNK_18022e1d8);
    }
  }
  else {
    uStack_68 = param_1[5];
    uVar3 = MapViewOfFile(param_1[4],0xf001f,0,0);
    uVar2 = GetLastError();
    param_1[6] = uVar3;
    cVar1 = FUN_18001dbc0(0);
    if (cVar1 != '\0') {
      puVar4 = param_2;
      if (0xf < (ulonglong)param_2[3]) {
        puVar4 = (undefined8 *)*param_2;
      }
      uVar3 = FUN_18001db50();
      uStack_58 = param_1[5];
      uStack_68 = CONCAT44(uStack_68._4_4_,0x14a);
      puStack_60 = puVar4;
      uStack_50 = uVar2;
      FUN_180015b40(uVar3,0,&UNK_18022e230,&UNK_18022e1d8);
    }
    if (param_1[6] != 0) goto LAB_1800f012f;
    cVar1 = FUN_18001dbc0(4);
    if (cVar1 != '\0') {
      uVar3 = FUN_18001db50();
      uStack_68 = CONCAT44(uStack_68._4_4_,0x14d);
      puVar5 = &UNK_18022e2b0;
      goto LAB_1800f0117;
    }
  }
  param_1[5] = 0;
LAB_1800f012f:
  param_1[7] = param_3;
  LOCK();
  _DAT_1802b11f4 = _DAT_1802b11f4 + 1;
  UNLOCK();
  cVar1 = FUN_18001dbc0(1);
  if (cVar1 != '\0') {
    if (0xf < (ulonglong)param_1[3]) {
      param_1 = (undefined8 *)*param_1;
    }
    uVar3 = FUN_18001db50();
    uStack_58 = CONCAT44(uStack_58._4_4_,_DAT_1802b11f4);
    uStack_68 = CONCAT44(uStack_68._4_4_,0x158);
    puStack_60 = param_1;
    FUN_180015b40(uVar3,1,&UNK_18022e300,&UNK_18022e1d8);
  }
  if (0xf < (ulonglong)param_2[3]) {
    __3_YAXPEAX_Z(*param_2);
  }
  param_2[3] = 0xf;
  param_2[2] = 0;
  *(undefined1 *)param_2 = 0;
  FUN_1801e8810(uStack_30 ^ (ulonglong)auStack_88);
  return;
}

