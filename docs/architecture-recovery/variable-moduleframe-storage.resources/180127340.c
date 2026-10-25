
void FUN_180127340(undefined8 param_1,int *param_2,char *param_3,longlong param_4,undefined4 param_5
                  ,undefined1 param_6)

{
  int iVar1;
  char cVar2;
  undefined8 uVar3;
  longlong lVar4;
  undefined1 auStack_78 [32];
  undefined4 uStack_58;
  undefined1 uStack_50;
  undefined8 uStack_48;
  undefined1 uStack_40;
  undefined7 uStack_3f;
  undefined8 uStack_30;
  ulonglong uStack_28;
  ulonglong uStack_20;
  
  uStack_48 = 0xfffffffffffffffe;
  uStack_20 = DAT_1802a5230 ^ (ulonglong)auStack_78;
  if (((param_2 != (int *)0x0) && (param_3 != (char *)0x0)) && (param_4 != 0)) {
    FUN_18008e650();
    iVar1 = *param_2;
    if (iVar1 == -1) {
      cVar2 = FUN_18001dbc0(4);
      if (cVar2 != '\0') {
        uVar3 = FUN_18001db50();
        uStack_58 = 0xa77;
        FUN_180015b40(uVar3,4,&DAT_180231fe8,&DAT_1802326f8);
      }
    }
    else {
      uStack_28 = 0xf;
      uStack_30 = 0;
      uStack_40 = 0;
      if (*param_3 != '\0') {
        lVar4 = -1;
        do {
          lVar4 = lVar4 + 1;
        } while (param_3[lVar4] != '\0');
      }
      FUN_180006300(&uStack_40,param_3);
      uVar3 = FUN_18008eca0();
      uStack_50 = param_6;
      uStack_58 = param_5;
      FUN_1800997c0(uVar3,iVar1,&uStack_40,param_4);
      if (0xf < uStack_28) {
        __3_YAXPEAX_Z(CONCAT71(uStack_3f,uStack_40));
      }
    }
  }
  FUN_1801e8810(uStack_20 ^ (ulonglong)auStack_78);
  return;
}

