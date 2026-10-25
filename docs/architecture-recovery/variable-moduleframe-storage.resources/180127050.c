
void FUN_180127050(undefined8 param_1,int *param_2,char *param_3,longlong param_4,undefined4 param_5
                  ,undefined1 param_6)

{
  int iVar1;
  char cVar2;
  undefined8 uVar3;
  longlong lVar4;
  undefined1 auStack_68 [32];
  undefined4 uStack_48;
  undefined1 uStack_40;
  undefined8 uStack_38;
  undefined1 uStack_30;
  undefined7 uStack_2f;
  undefined8 uStack_20;
  ulonglong uStack_18;
  ulonglong uStack_10;
  
  uStack_38 = 0xfffffffffffffffe;
  uStack_10 = DAT_1802a5230 ^ (ulonglong)auStack_68;
  if (((param_2 != (int *)0x0) && (param_3 != (char *)0x0)) && (param_4 != 0)) {
    FUN_18008e650();
    iVar1 = *param_2;
    if (iVar1 == -1) {
      cVar2 = FUN_18001dbc0(4);
      if (cVar2 != '\0') {
        uVar3 = FUN_18001db50();
        uStack_48 = 0xa53;
        FUN_180015b40(uVar3,4,&DAT_180231fe8,&DAT_180232750);
      }
    }
    else {
      uStack_18 = 0xf;
      uStack_20 = 0;
      uStack_30 = 0;
      if (*param_3 != '\0') {
        lVar4 = -1;
        do {
          lVar4 = lVar4 + 1;
        } while (param_3[lVar4] != '\0');
      }
      FUN_180006300(&uStack_30,param_3);
      uVar3 = FUN_18008eca0();
      uStack_40 = param_6;
      uStack_48 = param_5;
      FUN_180099550(uVar3,iVar1,&uStack_30,param_4);
      if (0xf < uStack_18) {
        __3_YAXPEAX_Z(CONCAT71(uStack_2f,uStack_30));
      }
    }
  }
  FUN_1801e8810(uStack_10 ^ (ulonglong)auStack_68);
  return;
}

