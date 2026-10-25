
void FUN_1801269f0(undefined8 param_1,int *param_2,char *param_3,undefined8 param_4,longlong param_5
                  )

{
  char cVar1;
  undefined8 uVar2;
  longlong lVar3;
  undefined1 auStack_78 [32];
  longlong lStack_58;
  undefined8 uStack_48;
  undefined1 uStack_40;
  undefined7 uStack_3f;
  undefined8 uStack_30;
  ulonglong uStack_28;
  ulonglong uStack_20;
  
  uStack_48 = 0xfffffffffffffffe;
  uStack_20 = DAT_1802a5230 ^ (ulonglong)auStack_78;
  if (((param_2 != (int *)0x0) && (param_3 != (char *)0x0)) && (param_5 != 0)) {
    FUN_18008e650();
    if (*param_2 == -1) {
      cVar1 = FUN_18001dbc0(4);
      if (cVar1 != '\0') {
        uVar2 = FUN_18001db50();
        lStack_58 = CONCAT44(lStack_58._4_4_,0x9fe);
        FUN_180015b40(uVar2,4,&DAT_180231fe8,&DAT_1802325d0);
      }
    }
    else {
      uStack_28 = 0xf;
      uStack_30 = 0;
      uStack_40 = 0;
      if (*param_3 != '\0') {
        lVar3 = -1;
        do {
          lVar3 = lVar3 + 1;
        } while (param_3[lVar3] != '\0');
      }
      FUN_180006300(&uStack_40,param_3);
      FUN_18008eca0();
      lStack_58 = param_5;
      FUN_180098360();
      if (0xf < uStack_28) {
        __3_YAXPEAX_Z(CONCAT71(uStack_3f,uStack_40));
      }
    }
  }
  FUN_1801e8810(uStack_20 ^ (ulonglong)auStack_78);
  return;
}

