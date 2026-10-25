
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800bb8f0(undefined8 param_1,undefined4 param_2,undefined8 param_3,undefined8 param_4,
                  longlong *param_5,float *param_6,undefined4 *param_7)

{
  longlong *plVar1;
  int iVar2;
  uint uVar3;
  longlong *plVar4;
  char cVar5;
  undefined4 uVar6;
  int iVar7;
  int iVar8;
  undefined8 uVar9;
  undefined8 *puVar10;
  int *piVar11;
  longlong lVar12;
  double dVar13;
  undefined1 auStack_d8 [32];
  float *pfStack_b8;
  undefined4 *puStack_b0;
  undefined4 auStack_a8 [2];
  longlong lStack_a0;
  longlong *plStack_98;
  longlong lStack_90;
  longlong *plStack_88;
  undefined8 uStack_80;
  undefined8 uStack_78;
  undefined1 uStack_70;
  undefined7 uStack_6f;
  undefined8 uStack_60;
  ulonglong uStack_58;
  undefined1 uStack_50;
  undefined7 uStack_4f;
  undefined8 uStack_40;
  ulonglong uStack_38;
  ulonglong uStack_30;
  
  uStack_78 = 0xfffffffffffffffe;
  uStack_30 = _DAT_1802a5230 ^ (ulonglong)auStack_d8;
  if ((param_6 == (float *)0x0) || (param_7 == (undefined4 *)0x0)) {
    cVar5 = FUN_18001dbc0(4);
    if (cVar5 != '\0') {
      uVar9 = FUN_18001db50();
      pfStack_b8 = (float *)CONCAT44(pfStack_b8._4_4_,0x16d);
      FUN_180015b40(uVar9,4,&UNK_1802246d0,&UNK_18022a670);
    }
  }
  else {
    lVar12 = 0;
    auStack_a8[0] = 0;
    piVar11 = (int *)*param_5;
    iVar2 = *piVar11;
    if (iVar2 == 1) {
      dVar13 = (double)atof(piVar11 + 1);
      *param_6 = (float)dVar13;
      auStack_a8[0] = 1;
    }
    else if (iVar2 == 2) {
      uVar9 = FUN_18008e5d0();
      FUN_18010a690(uVar9,&lStack_a0,param_2);
      if (lStack_a0 == 0) {
        cVar5 = FUN_18001dbc0(4);
        if (cVar5 != '\0') {
          uVar9 = FUN_18001db50();
          puStack_b0 = (undefined4 *)CONCAT44(puStack_b0._4_4_,param_2);
          pfStack_b8 = (float *)CONCAT44(pfStack_b8._4_4_,0x181);
          FUN_180015b40(uVar9,4,&UNK_18022a6a0,&UNK_18022a670);
        }
      }
      else {
        uVar6 = *(undefined4 *)(lStack_a0 + 0x28);
        uVar9 = FUN_18008e5d0();
        FUN_180108ba0(uVar9,&lStack_90,uVar6);
        if (lStack_90 == 0) {
          cVar5 = FUN_18001dbc0(4);
          if (cVar5 != '\0') {
            uVar9 = FUN_18001db50();
            puStack_b0 = (undefined4 *)CONCAT44(puStack_b0._4_4_,uVar6);
            pfStack_b8 = (float *)CONCAT44(pfStack_b8._4_4_,0x18a);
            FUN_180015b40(uVar9,4,&UNK_180224770,&UNK_18022a670);
          }
        }
        else {
          puVar10 = (undefined8 *)FUN_18001ec90(lStack_90,&uStack_50);
          if (0xf < (ulonglong)puVar10[3]) {
            puVar10 = (undefined8 *)*puVar10;
          }
          dVar13 = (double)atof(puVar10);
          *param_6 = (float)dVar13;
          if (0xf < uStack_38) {
            __3_YAXPEAX_Z(CONCAT71(uStack_4f,uStack_50));
          }
          uStack_38 = 0xf;
          uStack_40 = 0;
          uStack_50 = 0;
          auStack_a8[0] = 1;
        }
        plVar4 = plStack_88;
        if (plStack_88 != (longlong *)0x0) {
          LOCK();
          plVar1 = plStack_88 + 1;
          *(int *)plVar1 = (int)*plVar1 + -1;
          UNLOCK();
          if ((int)*plVar1 == 0) {
            (**(code **)*plStack_88)(plStack_88);
            LOCK();
            piVar11 = (int *)((longlong)plVar4 + 0xc);
            *piVar11 = *piVar11 + -1;
            UNLOCK();
            if (*piVar11 == 0) {
              (**(code **)(*plStack_88 + 8))();
            }
          }
        }
      }
      plVar4 = plStack_98;
      if (plStack_98 != (longlong *)0x0) {
        LOCK();
        plVar1 = plStack_98 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plStack_98)(plStack_98);
          LOCK();
          piVar11 = (int *)((longlong)plVar4 + 0xc);
          *piVar11 = *piVar11 + -1;
          UNLOCK();
          if (*piVar11 == 0) {
            (**(code **)(*plStack_98 + 8))();
          }
        }
      }
    }
    else if (iVar2 == 3) {
      iVar2 = piVar11[0x23];
      piVar11 = piVar11 + 0x24;
      uStack_58 = 0xf;
      uStack_60 = 0;
      uStack_70 = 0;
      if ((char)*piVar11 != '\0') {
        lVar12 = -1;
        do {
          lVar12 = lVar12 + 1;
        } while (*(char *)((longlong)piVar11 + lVar12) != '\0');
      }
      uVar9 = FUN_180006300(&uStack_70,piVar11,lVar12);
      uVar6 = FUN_1800bca20(uVar9,*param_5 + 4);
      uVar3 = *(uint *)(*param_5 + 4);
      cVar5 = FUN_1800bcd20(*param_5,iVar2);
      if (cVar5 == '\0') {
        uVar9 = FUN_18008e6c0(0);
        puStack_b0 = auStack_a8;
        pfStack_b8 = param_6;
        iVar8 = FUN_1800ca720(uVar9,iVar2,&uStack_70,uVar6);
      }
      else {
        uStack_80 = 0x1802b1208;
        FUN_1801e52d0(0x1802b1208);
        uVar9 = FUN_18008e910();
        puStack_b0 = auStack_a8;
        pfStack_b8 = param_6;
        iVar7 = FUN_1800ca720(uVar9,iVar2,&uStack_70,uVar6);
        iVar8 = 0;
        if (iVar7 != 0) {
          uVar9 = FUN_18008e6c0(0);
          puStack_b0 = auStack_a8;
          pfStack_b8 = param_6;
          iVar8 = FUN_1800ca720(uVar9,iVar2,&uStack_70,uVar6);
        }
        FUN_1801e5390(0x1802b1208);
      }
      if (iVar8 == 0) {
        if ((uVar3 & 0xfffffffd) == 0) {
          auStack_a8[0] = 1;
        }
      }
      else {
        cVar5 = FUN_18001dbc0(4);
        if (cVar5 != '\0') {
          uVar9 = FUN_18001db50();
          pfStack_b8 = (float *)CONCAT44(pfStack_b8._4_4_,0x1b2);
          FUN_180015b40(uVar9,4,&UNK_180226690,&UNK_18022a670);
        }
      }
      if (0xf < uStack_58) {
        __3_YAXPEAX_Z(CONCAT71(uStack_6f,uStack_70));
      }
    }
    else {
      cVar5 = FUN_18001dbc0(4);
      if (cVar5 != '\0') {
        uVar9 = FUN_18001db50();
        pfStack_b8 = (float *)CONCAT44(pfStack_b8._4_4_,0x1b9);
        FUN_180015b40(uVar9,4,&UNK_180226718,&UNK_18022a670);
      }
      auStack_a8[0] = 0;
    }
    *param_7 = auStack_a8[0];
  }
  FUN_1801e8810(uStack_30 ^ (ulonglong)auStack_d8);
  return;
}

