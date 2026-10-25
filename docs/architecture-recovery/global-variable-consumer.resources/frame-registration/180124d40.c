
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180124d40(undefined8 param_1,int *param_2,undefined8 param_3,undefined8 param_4)

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
  undefined1 auStack_f8 [32];
  undefined4 *puStack_d8;
  undefined1 *puStack_d0;
  int *piStack_c8;
  undefined4 uStack_b8;
  undefined1 auStack_b4 [4];
  undefined8 uStack_b0;
  undefined8 uStack_a8;
  undefined1 uStack_a0;
  undefined7 uStack_9f;
  undefined8 uStack_90;
  ulonglong uStack_88;
  undefined1 uStack_80;
  undefined7 uStack_7f;
  undefined8 uStack_70;
  ulonglong uStack_68;
  undefined1 uStack_60;
  undefined7 uStack_5f;
  undefined8 uStack_50;
  ulonglong uStack_48;
  ulonglong uStack_40;
  
  uStack_a8 = 0xfffffffffffffffe;
  uStack_40 = _DAT_1802a5230 ^ (ulonglong)auStack_f8;
  lVar8 = 0;
  uStack_b8 = 0;
  iVar2 = *param_2;
  if (((iVar2 != 0) && (iVar2 != 1)) && (iVar2 != 2)) {
    if (iVar2 == 3) {
      iVar2 = param_2[1];
      cVar3 = FUN_1800bcd20(0,iVar2);
      if (cVar3 == '\0') {
        piVar1 = param_2 + 2;
        uStack_48 = 0xf;
        uStack_50 = 0;
        uStack_60 = 0;
        if ((char)*piVar1 != '\0') {
          lVar8 = -1;
          do {
            lVar8 = lVar8 + 1;
          } while (*(char *)((longlong)piVar1 + lVar8) != '\0');
        }
        FUN_180006300(&uStack_60,piVar1,lVar8);
        uVar6 = FUN_18008e6c0(param_4);
        puStack_d0 = auStack_b4;
        puStack_d8 = &uStack_b8;
        iVar5 = FUN_1800ca500(uVar6,iVar2,&uStack_60,0);
        if (0xf < uStack_48) {
          __3_YAXPEAX_Z(CONCAT71(uStack_5f,uStack_60));
        }
        uStack_48 = 0xf;
        uStack_50 = 0;
        uStack_60 = 0;
      }
      else {
        uStack_b0 = 0x1802b1208;
        FUN_1801e52d0(0x1802b1208);
        piVar1 = param_2 + 2;
        uStack_88 = 0xf;
        uStack_90 = 0;
        uStack_a0 = 0;
        lVar7 = -1;
        lVar9 = lVar8;
        if ((char)*piVar1 != '\0') {
          lVar9 = -1;
          do {
            lVar9 = lVar9 + 1;
          } while (*(char *)((longlong)piVar1 + lVar9) != '\0');
        }
        FUN_180006300(&uStack_a0,piVar1,lVar9);
        uVar6 = FUN_18008e910();
        puStack_d0 = auStack_b4;
        puStack_d8 = &uStack_b8;
        iVar4 = FUN_1800ca500(uVar6,iVar2,&uStack_a0,0);
        if (0xf < uStack_88) {
          __3_YAXPEAX_Z(CONCAT71(uStack_9f,uStack_a0));
        }
        uStack_88 = 0xf;
        uStack_90 = 0;
        uStack_a0 = 0;
        iVar5 = 0;
        if (iVar4 != 0) {
          uStack_68 = 0xf;
          uStack_70 = 0;
          uStack_80 = 0;
          cVar3 = (char)*piVar1;
          while (cVar3 != '\0') {
            lVar7 = lVar7 + 1;
            lVar8 = lVar7;
            cVar3 = *(char *)((longlong)piVar1 + lVar7);
          }
          FUN_180006300(&uStack_80,piVar1,lVar8);
          uVar6 = FUN_18008e6c0(param_4);
          puStack_d0 = auStack_b4;
          puStack_d8 = &uStack_b8;
          iVar5 = FUN_1800ca500(uVar6,iVar2,&uStack_80,0);
          if (0xf < uStack_68) {
            __3_YAXPEAX_Z(CONCAT71(uStack_7f,uStack_80));
          }
          uStack_68 = 0xf;
          uStack_70 = 0;
          uStack_80 = 0;
        }
        FUN_1801e5390(0x1802b1208);
      }
      if ((iVar5 != 0) && (cVar3 = FUN_18001dbc0(4), cVar3 != '\0')) {
        uVar6 = FUN_18001db50();
        puStack_d0 = (undefined1 *)CONCAT44(puStack_d0._4_4_,param_2[1]);
        puStack_d8 = (undefined4 *)CONCAT44(puStack_d8._4_4_,0x87d);
        piStack_c8 = param_2 + 2;
        FUN_180015b40(uVar6,4,&UNK_180224a28,&UNK_180232338);
      }
    }
    else {
      cVar3 = FUN_18001dbc0(4);
      if (cVar3 != '\0') {
        uVar6 = FUN_18001db50();
        puStack_d8 = (undefined4 *)CONCAT44(puStack_d8._4_4_,0x885);
        FUN_180015b40(uVar6,4,&UNK_18021c668,&UNK_180232338);
      }
    }
  }
  FUN_1801e8810(uStack_40 ^ (ulonglong)auStack_f8);
  return;
}

