
void FUN_180127e60(undefined8 param_1,int *param_2,char *param_3)

{
  int iVar1;
  char cVar2;
  undefined8 uVar3;
  longlong lVar4;
  longlong lVar5;
  undefined1 auStack_88 [32];
  undefined4 uStack_68;
  undefined *puStack_58;
  undefined8 uStack_50;
  undefined1 uStack_48;
  undefined7 uStack_47;
  undefined8 uStack_38;
  ulonglong uStack_30;
  ulonglong uStack_28;
  
  uStack_50 = 0xfffffffffffffffe;
  uStack_28 = DAT_1802a5230 ^ (ulonglong)auStack_88;
  if ((param_2 != (int *)0x0) && (param_3 != (char *)0x0)) {
    if ((DAT_1802b2ac8 & 1) == 0) {
      DAT_1802b2ac8 = DAT_1802b2ac8 | 1;
      FUN_1800b5280();
      FUN_1801e8930(&DAT_1802029e0);
    }
    iVar1 = *param_2;
    if (iVar1 == -1) {
      cVar2 = FUN_18001dbc0(4);
      if (cVar2 != '\0') {
        uVar3 = FUN_18001db50();
        uStack_68 = 0xb24;
        FUN_180015b40(uVar3,4,&DAT_180231fe8,&DAT_1802328e0);
      }
    }
    else {
      puStack_58 = &DAT_1802b1208;
      FUN_1801e5430(&DAT_1802b1208);
      uStack_30 = 0xf;
      lVar4 = 0;
      uStack_38 = 0;
      uStack_48 = 0;
      lVar5 = lVar4;
      if (*param_3 != '\0') {
        lVar5 = -1;
        do {
          lVar5 = lVar5 + 1;
        } while (param_3[lVar5] != '\0');
      }
      FUN_180006300(&uStack_48,param_3,lVar5);
      uVar3 = FUN_18008e910();
      FUN_1800cb820(uVar3,iVar1,1,&uStack_48);
      if (0xf < uStack_30) {
        __3_YAXPEAX_Z(CONCAT71(uStack_47,uStack_48));
      }
      uStack_30 = 0xf;
      uStack_38 = 0;
      uStack_48 = 0;
      lVar5 = lVar4;
      if (*param_3 != '\0') {
        lVar5 = -1;
        do {
          lVar5 = lVar5 + 1;
        } while (param_3[lVar5] != '\0');
      }
      FUN_180006300(&uStack_48,param_3,lVar5);
      uVar3 = FUN_18008e910();
      FUN_1800cb820(uVar3,iVar1,2,&uStack_48);
      if (0xf < uStack_30) {
        __3_YAXPEAX_Z(CONCAT71(uStack_47,uStack_48));
      }
      uStack_30 = 0xf;
      uStack_38 = 0;
      uStack_48 = 0;
      lVar5 = lVar4;
      if (*param_3 != '\0') {
        lVar5 = -1;
        do {
          lVar5 = lVar5 + 1;
        } while (param_3[lVar5] != '\0');
      }
      FUN_180006300(&uStack_48,param_3,lVar5);
      uVar3 = FUN_18008e910();
      FUN_1800cb820(uVar3,iVar1,7,&uStack_48);
      if (0xf < uStack_30) {
        __3_YAXPEAX_Z(CONCAT71(uStack_47,uStack_48));
      }
      uStack_30 = 0xf;
      uStack_38 = 0;
      uStack_48 = 0;
      lVar5 = lVar4;
      if (*param_3 != '\0') {
        lVar5 = -1;
        do {
          lVar5 = lVar5 + 1;
        } while (param_3[lVar5] != '\0');
      }
      FUN_180006300(&uStack_48,param_3,lVar5);
      uVar3 = FUN_18008e910();
      FUN_1800cb820(uVar3,iVar1,3,&uStack_48);
      if (0xf < uStack_30) {
        __3_YAXPEAX_Z(CONCAT71(uStack_47,uStack_48));
      }
      uStack_30 = 0xf;
      uStack_38 = 0;
      uStack_48 = 0;
      lVar5 = lVar4;
      if (*param_3 != '\0') {
        lVar5 = -1;
        do {
          lVar5 = lVar5 + 1;
        } while (param_3[lVar5] != '\0');
      }
      FUN_180006300(&uStack_48,param_3,lVar5);
      uVar3 = FUN_18008e910();
      FUN_1800cb820(uVar3,iVar1,4,&uStack_48);
      if (0xf < uStack_30) {
        __3_YAXPEAX_Z(CONCAT71(uStack_47,uStack_48));
      }
      uStack_30 = 0xf;
      uStack_38 = 0;
      uStack_48 = 0;
      lVar5 = lVar4;
      if (*param_3 != '\0') {
        lVar5 = -1;
        do {
          lVar5 = lVar5 + 1;
        } while (param_3[lVar5] != '\0');
      }
      FUN_180006300(&uStack_48,param_3,lVar5);
      uVar3 = FUN_18008e910();
      FUN_1800cb820(uVar3,iVar1,5,&uStack_48);
      if (0xf < uStack_30) {
        __3_YAXPEAX_Z(CONCAT71(uStack_47,uStack_48));
      }
      uStack_30 = 0xf;
      uStack_38 = 0;
      uStack_48 = 0;
      if (*param_3 != '\0') {
        lVar4 = -1;
        do {
          lVar4 = lVar4 + 1;
        } while (param_3[lVar4] != '\0');
      }
      FUN_180006300(&uStack_48,param_3,lVar4);
      uVar3 = FUN_18008e910();
      FUN_1800cb820(uVar3,iVar1,6,&uStack_48);
      if (0xf < uStack_30) {
        __3_YAXPEAX_Z(CONCAT71(uStack_47,uStack_48));
      }
      FUN_1801e5390(&DAT_1802b1208);
    }
  }
  FUN_1801e8810(uStack_28 ^ (ulonglong)auStack_88);
  return;
}

