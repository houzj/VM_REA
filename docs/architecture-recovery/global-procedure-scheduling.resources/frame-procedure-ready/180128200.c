
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180128200(undefined8 param_1,int param_2,undefined8 *param_3,undefined1 param_4,
                  undefined4 param_5,undefined8 param_6,undefined1 param_7)

{
  int *piVar1;
  longlong *plVar2;
  longlong *plVar3;
  longlong *plVar4;
  ulonglong uVar5;
  char cVar6;
  int iVar7;
  int iVar8;
  undefined8 uVar9;
  undefined8 *puVar10;
  undefined8 *puVar11;
  longlong lVar12;
  longlong *plVar13;
  undefined1 auStack_1a8 [32];
  undefined8 uStack_188;
  undefined8 *puStack_180;
  int iStack_178;
  undefined8 *puStack_170;
  int iStack_168;
  char cStack_164;
  undefined1 uStack_163;
  undefined1 uStack_162;
  longlong *plStack_160;
  undefined8 uStack_158;
  longlong lStack_150;
  longlong *plStack_148;
  longlong lStack_140;
  longlong *plStack_138;
  undefined8 uStack_130;
  undefined1 uStack_128;
  undefined7 uStack_127;
  undefined8 uStack_118;
  ulonglong uStack_110;
  undefined1 uStack_108;
  undefined7 uStack_107;
  undefined8 uStack_f8;
  ulonglong uStack_f0;
  undefined1 uStack_e8;
  undefined7 uStack_e7;
  undefined8 uStack_d8;
  ulonglong uStack_d0;
  undefined8 auStack_c8 [3];
  ulonglong uStack_b0;
  undefined8 auStack_a8 [3];
  ulonglong uStack_90;
  undefined8 auStack_88 [3];
  ulonglong uStack_70;
  undefined8 auStack_68 [3];
  ulonglong uStack_50;
  ulonglong uStack_48;
  
  uStack_130 = 0xfffffffffffffffe;
  uStack_48 = _DAT_1802a5230 ^ (ulonglong)auStack_1a8;
  uStack_162 = param_4;
  if ((_DAT_1802b12c0 & 1) == 0) {
    _DAT_1802b12c0 = _DAT_1802b12c0 | 1;
    FUN_18003fde0();
    FUN_1801e8930(&UNK_180202970);
  }
  iVar7 = FUN_1800580c0(&DAT_1802b12d0);
  if (iVar7 == 0) {
    if (param_2 == 10999) {
      iVar7 = 0;
      uStack_158 = 0;
      plStack_160 = (longlong *)FUN_18001f150();
      if ((_DAT_1802b1888 & 1) == 0) {
        _DAT_1802b1888 = _DAT_1802b1888 | 1;
        FUN_180107fc0();
        FUN_1801e8930(&UNK_180202980);
      }
      iVar8 = FUN_180115330(&DAT_1802b1890,&plStack_160);
      if (iVar8 == 0) {
        uVar9 = FUN_18008e590();
        FUN_18006d750(uVar9);
        cStack_164 = '\0';
        puVar10 = param_3;
        if (0xf < (ulonglong)param_3[3]) {
          puVar10 = (undefined8 *)*param_3;
        }
        lVar12 = strstr(puVar10,&UNK_18022cb68);
        if (lVar12 != 0) {
          cStack_164 = '\x01';
          cVar6 = FUN_18001dbc0(2);
          if (cVar6 != '\0') {
            puVar10 = param_3;
            if (0xf < (ulonglong)param_3[3]) {
              puVar10 = (undefined8 *)*param_3;
            }
            uVar9 = FUN_18001db50();
            uStack_188 = CONCAT44(uStack_188._4_4_,0xb5e);
            puStack_180 = puVar10;
            FUN_180015b40(uVar9,2,&UNK_1802329a8,&UNK_180232850);
          }
          puVar10 = param_3;
          if (0xf < (ulonglong)param_3[3]) {
            puVar10 = (undefined8 *)*param_3;
          }
          FUN_18008fb90(0,puVar10 + 3);
        }
        uStack_163 = 0;
        iStack_168 = 0;
        plVar13 = (longlong *)*plStack_160;
        if (plVar13 != plStack_160) {
          do {
            plVar3 = (longlong *)plVar13[3];
            lVar12 = plVar13[2];
            if (plVar3 != (longlong *)0x0) {
              LOCK();
              *(int *)(plVar3 + 1) = (int)plVar3[1] + 1;
              UNLOCK();
            }
            lStack_140 = lVar12;
            plStack_138 = plVar3;
            if (lVar12 == 0) {
              cVar6 = FUN_18001dbc0(4);
              if (cVar6 != '\0') {
                uVar9 = FUN_18001db50();
                uStack_188 = CONCAT44(uStack_188._4_4_,0xb69);
                FUN_180015b40(uVar9,4,&UNK_180232908,&UNK_180232850);
              }
LAB_1801285a3:
              iVar7 = iStack_168;
              if (plVar3 != (longlong *)0x0) {
                LOCK();
                plVar4 = plVar3 + 1;
                *(int *)plVar4 = (int)*plVar4 + -1;
                UNLOCK();
                if ((int)*plVar4 == 0) {
                  (**(code **)*plVar3)(plVar3);
                  LOCK();
                  piVar1 = (int *)((longlong)plVar3 + 0xc);
                  *piVar1 = *piVar1 + -1;
                  UNLOCK();
                  iVar7 = iStack_168;
                  if (*piVar1 == 0) {
                    (**(code **)(*plVar3 + 8))(plVar3);
                    iVar7 = iStack_168;
                  }
                }
              }
            }
            else {
              iVar8 = *(int *)(lVar12 + 0x50);
              if ((iVar8 == 10999) || (iVar8 == 100000)) {
                if (plVar3 != (longlong *)0x0) {
                  LOCK();
                  plVar4 = plVar3 + 1;
                  *(int *)plVar4 = (int)*plVar4 + -1;
                  iVar8 = (int)*plVar4;
                  UNLOCK();
                  goto joined_r0x000180128682;
                }
              }
              else {
                iStack_168 = iVar7 + 1;
                if (*(char *)(lVar12 + 0xe8) == '\0') {
                  uVar9 = FUN_18008eda0();
                  cVar6 = FUN_1800e3fe0(uVar9,iVar8);
                  if (cVar6 == '\0') {
                    if (cStack_164 == '\0') {
                      puStack_180 = (undefined8 *)CONCAT71(puStack_180._1_7_,param_7);
                      uStack_188 = param_6;
                      iVar7 = FUN_1800dcc80(lVar12,uStack_162,param_5,param_3);
                    }
                    else {
                      uStack_110 = 0xf;
                      uStack_118 = 0;
                      uStack_128 = 0;
                      FUN_180006300(&uStack_128,&UNK_18022cb68,0x18);
                      puStack_180 = (undefined8 *)((ulonglong)puStack_180 & 0xffffffffffffff00);
                      uStack_188 = param_6;
                      iVar7 = FUN_1800dcc80(lVar12,uStack_162,param_5,&uStack_128);
                      if (0xf < uStack_110) {
                        __3_YAXPEAX_Z(CONCAT71(uStack_127,uStack_128));
                      }
                    }
                    if (iVar7 == 0) {
                      uStack_163 = 1;
                    }
                    else {
                      uVar9 = FUN_18008e590();
                      FUN_18006dd00(uVar9);
                      cVar6 = FUN_18001dbc0(4);
                      if (cVar6 != '\0') {
                        puVar10 = (undefined8 *)FUN_18008f750(auStack_68,iVar7);
                        puVar11 = (undefined8 *)FUN_1800dcb60(lVar12,&uStack_108);
                        if (0xf < (ulonglong)puVar10[3]) {
                          puVar10 = (undefined8 *)*puVar10;
                        }
                        if (0xf < (ulonglong)puVar11[3]) {
                          puVar11 = (undefined8 *)*puVar11;
                        }
                        uVar9 = FUN_18001db50();
                        iStack_178 = 10999;
                        uStack_188 = CONCAT44(uStack_188._4_4_,0xb8e);
                        puStack_180 = puVar11;
                        puStack_170 = puVar10;
                        FUN_180015b40(uVar9,4,&UNK_180232968,&UNK_180232850);
                        if (0xf < uStack_f0) {
                          __3_YAXPEAX_Z(CONCAT71(uStack_107,uStack_108));
                        }
                        uStack_f0 = 0xf;
                        uStack_f8 = 0;
                        uStack_108 = 0;
                        if (0xf < uStack_50) {
                          __3_YAXPEAX_Z(auStack_68[0]);
                        }
                      }
                    }
                    goto LAB_1801285a3;
                  }
                  cVar6 = FUN_18001dbc0(1);
                  if (cVar6 != '\0') {
                    puVar10 = (undefined8 *)FUN_1800dcb60(lVar12,auStack_a8);
                    iVar7 = *(int *)(lVar12 + 0x50);
                    if (0xf < (ulonglong)puVar10[3]) {
                      puVar10 = (undefined8 *)*puVar10;
                    }
                    uVar9 = FUN_18001db50();
                    uStack_188 = CONCAT44(uStack_188._4_4_,0xb7e);
                    puStack_180 = puVar10;
                    iStack_178 = iVar7;
                    FUN_180015b40(uVar9,1,&UNK_180232a40,&UNK_180232850);
                    uVar9 = auStack_a8[0];
                    uVar5 = uStack_90;
                    goto joined_r0x00018012871d;
                  }
                }
                else {
                  cVar6 = FUN_18001dbc0(2);
                  if (cVar6 != '\0') {
                    puVar10 = (undefined8 *)FUN_1800dcb60(lVar12,auStack_c8);
                    iVar7 = *(int *)(lVar12 + 0x50);
                    if (0xf < (ulonglong)puVar10[3]) {
                      puVar10 = (undefined8 *)*puVar10;
                    }
                    uVar9 = FUN_18001db50();
                    uStack_188 = CONCAT44(uStack_188._4_4_,0xb78);
                    puStack_180 = puVar10;
                    iStack_178 = iVar7;
                    FUN_180015b40(uVar9,2,&UNK_180232928,&UNK_180232850);
                    uVar9 = auStack_c8[0];
                    uVar5 = uStack_b0;
joined_r0x00018012871d:
                    if (0xf < uVar5) {
                      __3_YAXPEAX_Z(uVar9);
                    }
                  }
                }
                iVar7 = iStack_168;
                if (plVar3 != (longlong *)0x0) {
                  LOCK();
                  plVar4 = plVar3 + 1;
                  *(int *)plVar4 = (int)*plVar4 + -1;
                  iVar8 = (int)*plVar4;
                  UNLOCK();
joined_r0x000180128682:
                  if (iVar8 == 0) {
                    (**(code **)*plVar3)(plVar3);
                    LOCK();
                    piVar1 = (int *)((longlong)plVar3 + 0xc);
                    *piVar1 = *piVar1 + -1;
                    UNLOCK();
                    iVar7 = iStack_168;
                    if (*piVar1 == 0) {
                      (**(code **)(*plVar3 + 8))(plVar3);
                      iVar7 = iStack_168;
                    }
                  }
                }
              }
            }
            plVar13 = (longlong *)*plVar13;
          } while (plVar13 != plStack_160);
        }
      }
      plVar13 = (longlong *)*plStack_160;
      *plStack_160 = (longlong)plStack_160;
      plStack_160[1] = (longlong)plStack_160;
      if (plVar13 != plStack_160) {
        do {
          plVar3 = (longlong *)*plVar13;
          plVar4 = (longlong *)plVar13[3];
          if (plVar4 != (longlong *)0x0) {
            LOCK();
            plVar2 = plVar4 + 1;
            *(int *)plVar2 = (int)*plVar2 + -1;
            UNLOCK();
            if ((int)*plVar2 == 0) {
              (**(code **)*plVar4)(plVar4);
              LOCK();
              piVar1 = (int *)((longlong)plVar4 + 0xc);
              *piVar1 = *piVar1 + -1;
              UNLOCK();
              if (*piVar1 == 0) {
                (**(code **)(*plVar4 + 8))(plVar4);
              }
            }
          }
          __3_YAXPEAX_Z(plVar13);
          plVar13 = plVar3;
        } while (plVar3 != plStack_160);
      }
      __3_YAXPEAX_Z(plStack_160);
    }
    else {
      if ((_DAT_1802b1888 & 1) == 0) {
        _DAT_1802b1888 = _DAT_1802b1888 | 1;
        FUN_180107fc0();
        FUN_1801e8930(&UNK_180202980);
      }
      FUN_180108ba0(&DAT_1802b1890,&lStack_150,param_2);
      if (lStack_150 == 0) {
        cVar6 = FUN_18001dbc0(4);
        if (cVar6 != '\0') {
          uVar9 = FUN_18001db50();
          puStack_180 = (undefined8 *)CONCAT44(puStack_180._4_4_,param_2);
          uStack_188 = CONCAT44(uStack_188._4_4_,0xb44);
          FUN_180015b40(uVar9,4,&UNK_180232878,&UNK_180232850);
        }
      }
      else {
        puStack_180 = (undefined8 *)CONCAT71(puStack_180._1_7_,param_7);
        uStack_188 = param_6;
        iVar7 = FUN_1800dcc80(lStack_150,param_4,param_5,param_3);
        if ((iVar7 != 0) && (cVar6 = FUN_18001dbc0(4), cVar6 != '\0')) {
          puVar10 = (undefined8 *)FUN_18008f750(auStack_88,iVar7);
          puVar11 = (undefined8 *)FUN_1800dcb60(lStack_150,&uStack_e8);
          if (0xf < (ulonglong)puVar10[3]) {
            puVar10 = (undefined8 *)*puVar10;
          }
          if (0xf < (ulonglong)puVar11[3]) {
            puVar11 = (undefined8 *)*puVar11;
          }
          uVar9 = FUN_18001db50();
          uStack_188 = CONCAT44(uStack_188._4_4_,0xb4b);
          puStack_180 = puVar11;
          iStack_178 = param_2;
          puStack_170 = puVar10;
          FUN_180015b40(uVar9,4,&UNK_180232968,&UNK_180232850);
          if (0xf < uStack_d0) {
            __3_YAXPEAX_Z(CONCAT71(uStack_e7,uStack_e8));
          }
          uStack_d0 = 0xf;
          uStack_d8 = 0;
          uStack_e8 = 0;
          if (0xf < uStack_70) {
            __3_YAXPEAX_Z(auStack_88[0]);
          }
        }
      }
      plVar13 = plStack_148;
      if (plStack_148 != (longlong *)0x0) {
        LOCK();
        plVar3 = plStack_148 + 1;
        *(int *)plVar3 = (int)*plVar3 + -1;
        UNLOCK();
        if ((int)*plVar3 == 0) {
          (**(code **)*plStack_148)(plStack_148);
          LOCK();
          piVar1 = (int *)((longlong)plVar13 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (*piVar1 == 0) {
            (**(code **)(*plStack_148 + 8))();
          }
        }
      }
    }
  }
  FUN_1801e8810(uStack_48 ^ (ulonglong)auStack_1a8);
  return;
}

