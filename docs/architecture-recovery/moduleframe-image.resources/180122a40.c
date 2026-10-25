
void FUN_180122a40(int *param_1,char *param_2,undefined4 param_3,longlong param_4,longlong *param_5,
                  undefined8 param_6,char *param_7)

{
  int iVar1;
  char cVar2;
  int iVar3;
  undefined8 uVar4;
  undefined8 uVar5;
  longlong lVar6;
  longlong lVar7;
  undefined1 auStack_d8 [32];
  longlong lStack_b8;
  longlong *plStack_b0;
  char *pcStack_a8;
  longlong *plStack_a0;
  char *pcStack_98;
  longlong alStack_88 [2];
  undefined1 uStack_78;
  undefined7 uStack_77;
  undefined8 uStack_68;
  ulonglong uStack_60;
  ulonglong uStack_58;
  
  alStack_88[1] = 0xfffffffffffffffe;
  uStack_58 = DAT_1802a5230 ^ (ulonglong)auStack_d8;
  if ((param_2 != (char *)0x0) && (param_4 != 0)) {
    iVar1 = *param_1;
    uVar4 = *(undefined8 *)(param_1 + 2);
    if (iVar1 == -1) {
      cVar2 = FUN_18001dbc0(4);
      if (cVar2 != '\0') {
        uVar4 = FUN_18001db50();
        lStack_b8 = CONCAT44(lStack_b8._4_4_,0x634);
        FUN_180015b40(uVar4,4,&DAT_180231de8,&DAT_180231ed0);
      }
    }
    else {
      uVar5 = FUN_18008e5d0();
      cVar2 = FUN_180108b20(uVar5,iVar1);
      lVar6 = 0;
      lVar7 = param_4;
      if (cVar2 != '\0') {
        alStack_88[0] = 0;
        uVar5 = FUN_18008f040();
        plStack_a0 = alStack_88;
        pcStack_a8 = param_7;
        plStack_b0 = param_5;
        lStack_b8 = param_4;
        iVar3 = FUN_1800eee30(uVar5,iVar1,param_2,param_3);
        lVar7 = alStack_88[0];
        if ((iVar3 != 0) && (cVar2 = FUN_18001dbc0(4), lVar7 = param_4, cVar2 != '\0')) {
          uVar5 = FUN_18001db50();
          plStack_a0 = param_5;
          plStack_b0 = (longlong *)CONCAT44(plStack_b0._4_4_,iVar1);
          lStack_b8 = CONCAT44(lStack_b8._4_4_,0x642);
          pcStack_a8 = param_2;
          FUN_180015b40(uVar5,4,&DAT_180231ef0,&DAT_180231ed0);
        }
      }
      uStack_60 = 0xf;
      uStack_68 = 0;
      uStack_78 = 0;
      if (*param_2 != '\0') {
        lVar6 = -1;
        do {
          lVar6 = lVar6 + 1;
        } while (param_2[lVar6] != '\0');
      }
      FUN_180006300(&uStack_78,param_2,lVar6);
      uVar4 = FUN_18008e6c0(uVar4);
      pcStack_98 = param_7;
      plStack_b0 = param_5;
      lStack_b8 = lVar7;
      FUN_1800c99f0(uVar4,iVar1,&uStack_78,param_3);
      if (0xf < uStack_60) {
        __3_YAXPEAX_Z(CONCAT71(uStack_77,uStack_78));
      }
    }
  }
  FUN_1801e8810(uStack_58 ^ (ulonglong)auStack_d8);
  return;
}

