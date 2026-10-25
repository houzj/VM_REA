
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_18011e580(undefined8 param_1,int *param_2,longlong *param_3,undefined4 param_4,
                  undefined4 *param_5,longlong *param_6)

{
  longlong *plVar1;
  int *piVar2;
  int iVar3;
  int iVar4;
  undefined4 uVar5;
  longlong *plVar6;
  char cVar7;
  undefined4 uVar8;
  int iVar9;
  int iVar10;
  undefined8 uVar11;
  undefined8 *puVar12;
  undefined1 *puVar13;
  int iVar14;
  longlong lVar15;
  undefined1 auStack_1b8 [32];
  undefined4 *puStack_198;
  longlong *plStack_190;
  longlong *plStack_188;
  undefined4 uStack_180;
  int iStack_178;
  undefined1 *puStack_170;
  undefined4 uStack_168;
  undefined8 *puStack_160;
  undefined4 uStack_158;
  undefined4 uStack_154;
  undefined4 uStack_150;
  longlong lStack_148;
  longlong *plStack_140;
  longlong lStack_138;
  longlong *plStack_130;
  int *piStack_128;
  longlong *plStack_120;
  longlong *plStack_118;
  undefined8 uStack_110;
  undefined8 uStack_108;
  undefined1 uStack_100;
  undefined7 uStack_ff;
  undefined8 uStack_f0;
  ulonglong uStack_e8;
  undefined1 uStack_e0;
  undefined7 uStack_df;
  undefined8 uStack_d0;
  ulonglong uStack_c8;
  undefined1 uStack_c0;
  undefined7 uStack_bf;
  undefined8 uStack_b0;
  ulonglong uStack_a8;
  undefined1 uStack_a0;
  undefined7 uStack_9f;
  undefined8 uStack_90;
  ulonglong uStack_88;
  undefined1 uStack_80;
  undefined7 uStack_7f;
  undefined8 uStack_70;
  ulonglong uStack_68;
  undefined8 auStack_60 [3];
  ulonglong uStack_48;
  ulonglong uStack_40;
  
  uStack_108 = 0xfffffffffffffffe;
  uStack_40 = _DAT_1802a5230 ^ (ulonglong)auStack_1b8;
  plStack_118 = param_6;
  uStack_150 = param_4;
  if (((param_3 != (longlong *)0x0) && (param_5 != (undefined4 *)0x0)) &&
     (param_6 != (longlong *)0x0)) {
    iVar3 = *param_2;
    lStack_148 = *(longlong *)(param_2 + 2);
    if (iVar3 == -1) {
      cVar7 = FUN_18001dbc0(4);
      if (cVar7 != '\0') {
        uVar11 = FUN_18001db50();
        puStack_198 = (undefined4 *)CONCAT44(puStack_198._4_4_,0x15b);
        plStack_190 = param_3;
        FUN_180015b40(uVar11,4,&UNK_180231810,&UNK_180231a18);
      }
    }
    else {
      uStack_c8 = 0xf;
      uStack_d0 = 0;
      uStack_e0 = 0;
      if ((char)*param_3 == '\0') {
        lVar15 = 0;
      }
      else {
        lVar15 = -1;
        do {
          lVar15 = lVar15 + 1;
        } while (*(char *)((longlong)param_3 + lVar15) != '\0');
      }
      FUN_180006300(&uStack_e0,param_3,lVar15);
      cVar7 = FUN_18012cd50();
      if (cVar7 == '\0') {
        uVar11 = FUN_18008e9a0();
        FUN_1800fb040(uVar11,&piStack_128,iVar3,&uStack_e0);
        if (piStack_128 == (int *)0x0) {
          cVar7 = FUN_18001dbc0(0);
          if (cVar7 != '\0') {
            plStack_190 = (longlong *)FUN_18008f750(auStack_60,param_2);
            if (0xf < (ulonglong)plStack_190[3]) {
              plStack_190 = (longlong *)*plStack_190;
            }
            uVar11 = FUN_18001db50();
            puStack_198 = (undefined4 *)CONCAT44(puStack_198._4_4_,0x168);
            plStack_188 = param_3;
            FUN_180015b40(uVar11,0,&UNK_180231860,&UNK_180231a18);
            if (0xf < uStack_48) {
              __3_YAXPEAX_Z(auStack_60[0]);
            }
          }
        }
        else {
          lVar15 = 0;
          iVar14 = 0;
          uStack_158 = 0;
          iVar4 = *piStack_128;
          if (iVar4 == 1) {
            cVar7 = FUN_18012d030(0,piStack_128 + 1);
            if (cVar7 == '\0') {
              cVar7 = FUN_18001dbc0(1);
              if (cVar7 != '\0') {
                uVar11 = FUN_18001db50();
                plStack_190 = (longlong *)CONCAT44(plStack_190._4_4_,iVar3);
                puStack_198 = (undefined4 *)CONCAT44(puStack_198._4_4_,0x17d);
                plStack_188 = param_3;
                FUN_180015b40(uVar11,1,&UNK_180231a40,&UNK_180231a18);
              }
            }
            else {
              uVar8 = atoi(piStack_128 + 1);
              *param_5 = uVar8;
              uStack_158 = 1;
            }
          }
          else {
            uVar8 = 0;
            if (iVar4 == 2) {
              uVar11 = FUN_18008e5d0();
              FUN_18010a690(uVar11,&lStack_148,iVar3);
              if (lStack_148 == 0) {
                cVar7 = FUN_18001dbc0(4);
                if (cVar7 != '\0') {
                  uVar11 = FUN_18001db50();
                  plStack_190 = (longlong *)CONCAT44(plStack_190._4_4_,iVar3);
                  puStack_198 = (undefined4 *)CONCAT44(puStack_198._4_4_,0x186);
                  plStack_188 = param_3;
                  FUN_180015b40(uVar11,4,&UNK_1802319c0,&UNK_180231a18);
                }
              }
              else {
                uVar5 = *(undefined4 *)(lStack_148 + 0x28);
                uVar11 = FUN_18008e5d0();
                FUN_180108ba0(uVar11,&lStack_138,uVar5);
                if (lStack_138 == 0) {
                  cVar7 = FUN_18001dbc0(4);
                  if (cVar7 != '\0') {
                    uVar11 = FUN_18001db50();
                    plStack_190 = (longlong *)CONCAT44(plStack_190._4_4_,uVar5);
                    puStack_198 = (undefined4 *)CONCAT44(puStack_198._4_4_,399);
                    plStack_188 = param_3;
                    FUN_180015b40(uVar11,4,&UNK_1802318c0,&UNK_180231a18);
                  }
                }
                else {
                  puVar12 = (undefined8 *)FUN_18001ec90(lStack_138,&uStack_a0);
                  if (0xf < (ulonglong)puVar12[3]) {
                    puVar12 = (undefined8 *)*puVar12;
                  }
                  cVar7 = FUN_18012d030();
                  if (cVar7 == '\0') {
                    iVar14 = -0x1fffffff;
                  }
                  else {
                    uVar8 = atoi(puVar12);
                  }
                  if (0xf < uStack_88) {
                    __3_YAXPEAX_Z(CONCAT71(uStack_9f,uStack_a0));
                  }
                  uStack_88 = 0xf;
                  uStack_90 = 0;
                  uStack_a0 = 0;
                  if (iVar14 == 0) {
                    *param_5 = uVar8;
                    uStack_158 = 1;
                  }
                  else {
                    cVar7 = FUN_18001dbc0(1);
                    if (cVar7 != '\0') {
                      uVar11 = FUN_18001db50();
                      plStack_190 = (longlong *)CONCAT44(plStack_190._4_4_,iVar3);
                      puStack_198 = (undefined4 *)CONCAT44(puStack_198._4_4_,0x19d);
                      plStack_188 = param_3;
                      FUN_180015b40(uVar11,1,&UNK_180231b70,&UNK_180231a18);
                    }
                  }
                }
                plVar6 = plStack_130;
                if (plStack_130 != (longlong *)0x0) {
                  LOCK();
                  plVar1 = plStack_130 + 1;
                  *(int *)plVar1 = (int)*plVar1 + -1;
                  UNLOCK();
                  if ((int)*plVar1 == 0) {
                    (**(code **)*plStack_130)(plStack_130);
                    LOCK();
                    piVar2 = (int *)((longlong)plVar6 + 0xc);
                    *piVar2 = *piVar2 + -1;
                    UNLOCK();
                    if (*piVar2 == 0) {
                      (**(code **)(*plStack_130 + 8))();
                    }
                  }
                }
              }
              plVar6 = plStack_140;
              if (plStack_140 != (longlong *)0x0) {
                LOCK();
                plVar1 = plStack_140 + 1;
                *(int *)plVar1 = (int)*plVar1 + -1;
                UNLOCK();
                if ((int)*plVar1 == 0) {
                  (**(code **)*plStack_140)(plStack_140);
                  LOCK();
                  piVar2 = (int *)((longlong)plVar6 + 0xc);
                  *piVar2 = *piVar2 + -1;
                  UNLOCK();
                  if (*piVar2 == 0) {
                    (**(code **)(*plStack_140 + 8))();
                  }
                }
              }
            }
            else if (iVar4 == 3) {
              iVar4 = piStack_128[0x23];
              piVar2 = piStack_128 + 0x24;
              uStack_e8 = 0xf;
              uStack_f0 = 0;
              uStack_100 = 0;
              if ((char)*piVar2 != '\0') {
                lVar15 = -1;
                do {
                  lVar15 = lVar15 + 1;
                } while (*(char *)((longlong)piVar2 + lVar15) != '\0');
              }
              FUN_180006300(&uStack_100,piVar2,lVar15);
              uVar8 = FUN_180124d40();
              iVar14 = piStack_128[1];
              uStack_154 = uVar8;
              cVar7 = FUN_1800bcd20();
              if (cVar7 == '\0') {
                uVar11 = FUN_18008e6c0(lStack_148);
                plStack_190 = (longlong *)&uStack_158;
                puStack_198 = param_5;
                iVar10 = FUN_1800ca500(uVar11,iVar4,&uStack_100,uVar8);
              }
              else {
                uStack_110 = 0x1802b1208;
                FUN_1801e52d0(0x1802b1208);
                uVar11 = FUN_18008e910();
                plStack_190 = (longlong *)&uStack_158;
                puStack_198 = param_5;
                iVar9 = FUN_1800ca500(uVar11,iVar4,&uStack_100,uVar8);
                iVar10 = 0;
                if (iVar9 != 0) {
                  uVar11 = FUN_18008e6c0(lStack_148);
                  plStack_190 = (longlong *)&uStack_158;
                  puStack_198 = param_5;
                  iVar10 = FUN_1800ca500(uVar11,iVar4,&uStack_100,uStack_154);
                }
                FUN_1801e5390(0x1802b1208);
              }
              if (iVar10 == 0) {
                if (iVar14 != 1) {
                  uStack_158 = 1;
                }
              }
              else if (iVar10 == -0x1ffffcec) {
                cVar7 = FUN_18001dbc0(1);
                if (cVar7 != '\0') {
                  puStack_160 = (undefined8 *)FUN_18008f750(&uStack_80,0xe0000314);
                  if (0xf < (ulonglong)puStack_160[3]) {
                    puStack_160 = (undefined8 *)*puStack_160;
                  }
                  puVar13 = &uStack_100;
                  if (0xf < uStack_e8) {
                    puVar13 = (undefined1 *)CONCAT71(uStack_ff,uStack_100);
                  }
                  uVar11 = FUN_18001db50();
                  uStack_168 = uStack_154;
                  uStack_180 = uStack_150;
                  plStack_190 = (longlong *)CONCAT44(plStack_190._4_4_,iVar3);
                  puStack_198 = (undefined4 *)CONCAT44(puStack_198._4_4_,0x1c3);
                  plStack_188 = param_3;
                  iStack_178 = iVar4;
                  puStack_170 = puVar13;
                  FUN_180015b40(uVar11,1,&UNK_180224800,&UNK_180231a18);
                  if (0xf < uStack_68) {
                    __3_YAXPEAX_Z(CONCAT71(uStack_7f,uStack_80));
                  }
                  uStack_68 = 0xf;
                  uStack_70 = 0;
                  uStack_80 = 0;
                }
              }
              else {
                cVar7 = FUN_18001dbc0(4);
                if (cVar7 != '\0') {
                  puStack_160 = (undefined8 *)FUN_18008f750(&uStack_c0,iVar10);
                  if (0xf < (ulonglong)puStack_160[3]) {
                    puStack_160 = (undefined8 *)*puStack_160;
                  }
                  puVar13 = &uStack_100;
                  if (0xf < uStack_e8) {
                    puVar13 = (undefined1 *)CONCAT71(uStack_ff,uStack_100);
                  }
                  uVar11 = FUN_18001db50();
                  uStack_168 = uStack_154;
                  uStack_180 = uStack_150;
                  plStack_190 = (longlong *)CONCAT44(plStack_190._4_4_,iVar3);
                  puStack_198 = (undefined4 *)CONCAT44(puStack_198._4_4_,0x1c8);
                  plStack_188 = param_3;
                  iStack_178 = iVar4;
                  puStack_170 = puVar13;
                  FUN_180015b40(uVar11,4,&UNK_180224800,&UNK_180231a18);
                  if (0xf < uStack_a8) {
                    __3_YAXPEAX_Z(CONCAT71(uStack_bf,uStack_c0));
                  }
                  uStack_a8 = 0xf;
                  uStack_b0 = 0;
                  uStack_c0 = 0;
                }
              }
              if (0xf < uStack_e8) {
                __3_YAXPEAX_Z(CONCAT71(uStack_ff,uStack_100));
              }
            }
            else {
              cVar7 = FUN_18001dbc0(4);
              if (cVar7 != '\0') {
                uVar11 = FUN_18001db50();
                puStack_198 = (undefined4 *)CONCAT44(puStack_198._4_4_,0x1d0);
                plStack_190 = param_3;
                FUN_180015b40(uVar11,4,&UNK_180224860,&UNK_180231a18);
              }
              uStack_158 = 0;
            }
          }
          *(undefined4 *)plStack_118 = uStack_158;
        }
        plVar6 = plStack_120;
        if (plStack_120 != (longlong *)0x0) {
          LOCK();
          plVar1 = plStack_120 + 1;
          *(int *)plVar1 = (int)*plVar1 + -1;
          UNLOCK();
          if ((int)*plVar1 == 0) {
            (**(code **)*plStack_120)(plStack_120);
            LOCK();
            piVar2 = (int *)((longlong)plVar6 + 0xc);
            *piVar2 = *piVar2 + -1;
            UNLOCK();
            if (*piVar2 == 0) {
              (**(code **)(*plStack_120 + 8))();
            }
          }
        }
      }
      else {
        uVar11 = FUN_18008e960();
        plStack_190 = param_6;
        puStack_198 = param_5;
        FUN_1800ca500(uVar11,iVar3,&uStack_e0,param_4);
      }
      if (0xf < uStack_c8) {
        __3_YAXPEAX_Z(CONCAT71(uStack_df,uStack_e0));
      }
    }
  }
  FUN_1801e8810(uStack_40 ^ (ulonglong)auStack_1b8);
  return;
}

