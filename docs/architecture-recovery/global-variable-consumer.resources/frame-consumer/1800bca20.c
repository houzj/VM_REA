
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800bca20(undefined8 param_1,int *param_2)

{
  int *piVar1;
  int iVar2;
  char cVar3;
  int iVar4;
  int iVar5;
  undefined8 uVar6;
  longlong lVar7;
  longlong lVar8;
  longlong lVar9;
  undefined1 auStack_e8 [32];
  undefined4 *puStack_c8;
  undefined1 *puStack_c0;
  int *piStack_b8;
  undefined4 uStack_a8;
  undefined1 auStack_a4 [4];
  undefined8 uStack_a0;
  undefined8 uStack_98;
  undefined1 uStack_90;
  undefined7 uStack_8f;
  undefined8 uStack_80;
  ulonglong uStack_78;
  undefined1 uStack_70;
  undefined7 uStack_6f;
  undefined8 uStack_60;
  ulonglong uStack_58;
  undefined1 uStack_50;
  undefined7 uStack_4f;
  undefined8 uStack_40;
  ulonglong uStack_38;
  ulonglong uStack_30;
  
  uStack_98 = 0xfffffffffffffffe;
  uStack_30 = _DAT_1802a5230 ^ (ulonglong)auStack_e8;
  lVar8 = 0;
  uStack_a8 = 0;
  iVar2 = *param_2;
  if (((iVar2 != 0) && (iVar2 != 1)) && (iVar2 != 2)) {
    if (iVar2 == 3) {
      iVar2 = param_2[1];
      cVar3 = FUN_1800bcd20(0,iVar2);
      if (cVar3 == '\0') {
        piVar1 = param_2 + 2;
        uStack_38 = 0xf;
        uStack_40 = 0;
        uStack_50 = 0;
        if ((char)*piVar1 != '\0') {
          lVar8 = -1;
          do {
            lVar8 = lVar8 + 1;
          } while (*(char *)((longlong)piVar1 + lVar8) != '\0');
        }
        FUN_180006300(&uStack_50,piVar1,lVar8);
        uVar6 = FUN_18008e6c0(0);
        puStack_c0 = auStack_a4;
        puStack_c8 = &uStack_a8;
        iVar5 = FUN_1800ca500(uVar6,iVar2,&uStack_50,0);
        if (0xf < uStack_38) {
          __3_YAXPEAX_Z(CONCAT71(uStack_4f,uStack_50));
        }
        uStack_38 = 0xf;
        uStack_40 = 0;
        uStack_50 = 0;
      }
      else {
        uStack_a0 = 0x1802b1208;
        FUN_1801e52d0(0x1802b1208);
        piVar1 = param_2 + 2;
        uStack_78 = 0xf;
        uStack_80 = 0;
        uStack_90 = 0;
        lVar7 = -1;
        lVar9 = lVar8;
        if ((char)*piVar1 != '\0') {
          lVar9 = -1;
          do {
            lVar9 = lVar9 + 1;
          } while (*(char *)((longlong)piVar1 + lVar9) != '\0');
        }
        FUN_180006300(&uStack_90,piVar1,lVar9);
        uVar6 = FUN_18008e910();
        puStack_c0 = auStack_a4;
        puStack_c8 = &uStack_a8;
        iVar4 = FUN_1800ca500(uVar6,iVar2,&uStack_90,0);
        if (0xf < uStack_78) {
          __3_YAXPEAX_Z(CONCAT71(uStack_8f,uStack_90));
        }
        uStack_78 = 0xf;
        uStack_80 = 0;
        uStack_90 = 0;
        iVar5 = 0;
        if (iVar4 != 0) {
          uStack_58 = 0xf;
          uStack_60 = 0;
          uStack_70 = 0;
          cVar3 = (char)*piVar1;
          while (cVar3 != '\0') {
            lVar7 = lVar7 + 1;
            lVar8 = lVar7;
            cVar3 = *(char *)((longlong)piVar1 + lVar7);
          }
          FUN_180006300(&uStack_70,piVar1,lVar8);
          uVar6 = FUN_18008e6c0(0);
          puStack_c0 = auStack_a4;
          puStack_c8 = &uStack_a8;
          iVar5 = FUN_1800ca500(uVar6,iVar2,&uStack_70,0);
          if (0xf < uStack_58) {
            __3_YAXPEAX_Z(CONCAT71(uStack_6f,uStack_70));
          }
          uStack_58 = 0xf;
          uStack_60 = 0;
          uStack_70 = 0;
        }
        FUN_1801e5390(0x1802b1208);
      }
      if ((iVar5 != 0) && (cVar3 = FUN_18001dbc0(4), cVar3 != '\0')) {
        uVar6 = FUN_18001db50();
        puStack_c0 = (undefined1 *)CONCAT44(puStack_c0._4_4_,param_2[1]);
        puStack_c8 = (undefined4 *)CONCAT44(puStack_c8._4_4_,0x2fe);
        piStack_b8 = param_2 + 2;
        FUN_180015b40(uVar6,4,&UNK_180224a28,&UNK_18022a7c8);
      }
    }
    else {
      cVar3 = FUN_18001dbc0(4);
      if (cVar3 != '\0') {
        uVar6 = FUN_18001db50();
        puStack_c8 = (undefined4 *)CONCAT44(puStack_c8._4_4_,0x306);
        FUN_180015b40(uVar6,4,&UNK_18021c668,&UNK_18022a7c8);
      }
    }
  }
  FUN_1801e8810(uStack_30 ^ (ulonglong)auStack_e8);
  return;
}

