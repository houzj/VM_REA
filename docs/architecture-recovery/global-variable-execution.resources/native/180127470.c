
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180127470(undefined8 param_1,int *param_2,char *param_3,undefined4 param_4,longlong param_5
                  ,undefined8 param_6)

{
  int iVar1;
  char cVar2;
  undefined8 uVar3;
  longlong lVar4;
  undefined1 auStack_98 [32];
  longlong lStack_78;
  undefined8 uStack_70;
  undefined8 uStack_68;
  undefined8 uStack_60;
  undefined1 uStack_58;
  undefined7 uStack_57;
  undefined8 uStack_48;
  ulonglong uStack_40;
  ulonglong uStack_38;
  
  uStack_60 = 0xfffffffffffffffe;
  uStack_38 = _DAT_1802a5230 ^ (ulonglong)auStack_98;
  if ((param_2 != (int *)0x0) && (param_3 != (char *)0x0)) {
    if ((_DAT_1802b2ac8 & 1) == 0) {
      _DAT_1802b2ac8 = _DAT_1802b2ac8 | 1;
      FUN_1800b5280();
      FUN_1801e8930(&UNK_1802029e0);
    }
    iVar1 = *param_2;
    if (iVar1 == -1) {
      cVar2 = FUN_18001dbc0(4);
      if (cVar2 != '\0') {
        uVar3 = FUN_18001db50();
        lStack_78 = CONCAT44(lStack_78._4_4_,0xa8a);
        FUN_180015b40(uVar3,4,&UNK_180231fe8,&UNK_180232728);
      }
    }
    else {
      uStack_68 = 0x1802b1208;
      FUN_1801e52d0(0x1802b1208);
      uStack_40 = 0xf;
      uStack_48 = 0;
      uStack_58 = 0;
      if (*param_3 != '\0') {
        lVar4 = -1;
        do {
          lVar4 = lVar4 + 1;
        } while (param_3[lVar4] != '\0');
      }
      FUN_180006300(&uStack_58,param_3);
      uVar3 = FUN_18008e910();
      if (param_5 != 0) {
        uStack_70 = param_6;
        lStack_78 = param_5;
        FUN_1800ca500(uVar3,iVar1,&uStack_58,param_4);
      }
      if (0xf < uStack_40) {
        __3_YAXPEAX_Z(CONCAT71(uStack_57,uStack_58));
      }
      FUN_1801e5390(0x1802b1208);
    }
  }
  FUN_1801e8810(uStack_38 ^ (ulonglong)auStack_98);
  return;
}

