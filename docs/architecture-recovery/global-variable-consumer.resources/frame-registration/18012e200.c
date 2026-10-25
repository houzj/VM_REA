
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_18012e200(undefined8 param_1,int *param_2,int param_3,longlong *param_4,int *param_5)

{
  longlong *plVar1;
  int *piVar2;
  ulonglong uVar3;
  char cVar4;
  int iVar5;
  int iVar6;
  undefined8 uVar7;
  undefined8 *puVar8;
  int *piVar9;
  longlong lVar10;
  longlong *plVar11;
  undefined4 uVar12;
  undefined1 auStack_1c8 [32];
  longlong *plStack_1a8;
  undefined8 uStack_1a0;
  longlong *plStack_198;
  int iStack_190;
  longlong lStack_188;
  int iStack_178;
  int iStack_174;
  int *piStack_170;
  longlong *plStack_168;
  int iStack_160;
  longlong lStack_158;
  longlong *plStack_150;
  undefined8 uStack_148;
  longlong *plStack_140;
  undefined8 uStack_138;
  longlong *plStack_130;
  longlong lStack_128;
  longlong *plStack_120;
  longlong lStack_118;
  longlong *plStack_110;
  undefined8 uStack_108;
  undefined8 uStack_100;
  undefined1 auStack_f8 [8];
  longlong *plStack_f0;
  longlong *plStack_e0;
  undefined1 auStack_d8 [8];
  longlong *plStack_d0;
  undefined1 auStack_c8 [8];
  longlong *plStack_c0;
  undefined1 auStack_b8 [8];
  longlong *plStack_b0;
  undefined8 uStack_a8;
  undefined1 auStack_a0 [8];
  longlong *plStack_98;
  undefined8 uStack_90;
  undefined8 uStack_88;
  undefined8 auStack_80 [2];
  undefined8 uStack_70;
  ulonglong uStack_68;
  undefined1 uStack_60;
  undefined7 uStack_5f;
  undefined8 uStack_50;
  ulonglong uStack_48;
  ulonglong uStack_40;
  
  uStack_88 = 0xfffffffffffffffe;
  uStack_40 = _DAT_1802a5230 ^ (ulonglong)auStack_1c8;
  iVar6 = *param_2;
  uVar7 = *(undefined8 *)(param_2 + 2);
  iStack_178 = param_3;
  uStack_100 = uVar7;
  if (iVar6 == -1) {
    cVar4 = FUN_18001dbc0(4);
    if (cVar4 != '\0') {
      if (0xf < (ulonglong)param_4[3]) {
        param_4 = (longlong *)*param_4;
      }
      uVar7 = FUN_18001db50();
      plStack_1a8 = (longlong *)CONCAT44(plStack_1a8._4_4_,0x1040);
      uStack_1a0 = param_4;
      FUN_180015b40(uVar7,4,&UNK_180231810,&UNK_1802335d0);
    }
    goto LAB_18012ede7;
  }
  if ((_DAT_1802b2cd8 & 1) == 0) {
    _DAT_1802b2cd8 = _DAT_1802b2cd8 | 1;
    FUN_1800facf0();
    FUN_1801e8930(FUN_180202b30);
  }
  FUN_1800fb040(&DAT_1802b2ce0,&piStack_170,iVar6,param_4);
  piVar9 = piStack_170;
  if (piStack_170 == (int *)0x0) {
    cVar4 = FUN_18001dbc0(1);
    if (cVar4 != '\0') {
      if (0xf < (ulonglong)param_4[3]) {
        param_4 = (longlong *)*param_4;
      }
      uVar7 = FUN_18001db50();
      uStack_1a0 = (longlong *)CONCAT44(uStack_1a0._4_4_,iVar6);
      plStack_1a8 = (longlong *)CONCAT44(plStack_1a8._4_4_,0x1047);
      plStack_198 = param_4;
      FUN_180015b40(uVar7,1,&UNK_180233530,&UNK_1802335d0);
    }
  }
  else {
    uVar7 = FUN_18008e6c0(uVar7);
    uVar12 = FUN_1800cae00(uVar7,auStack_f8,iVar6);
    plVar11 = (longlong *)(param_5 + 2);
    plStack_1a8 = plVar11;
    iVar5 = FUN_18012dcb0(uVar12,param_3,param_4,auStack_f8);
    if (iVar5 == 0) {
      *param_5 = iStack_178;
      if (plVar11 != param_4) {
        FUN_180006470(plVar11,param_4,0,0xffffffffffffffff);
      }
      param_5[0x26] = *piVar9;
      uStack_68 = 0xf;
      lVar10 = 0;
      uStack_70 = 0;
      auStack_80[0]._0_1_ = 0;
      iVar5 = *piVar9;
      if (iVar5 < 1) {
LAB_18012ed1a:
        cVar4 = FUN_18001dbc0(4);
        if (cVar4 != '\0') {
          if (0xf < (ulonglong)param_4[3]) {
            param_4 = (longlong *)*param_4;
          }
          uVar7 = FUN_18001db50();
          plStack_1a8 = (longlong *)CONCAT44(plStack_1a8._4_4_,0x110e);
          uStack_1a0 = param_4;
          FUN_180015b40(uVar7,4,&UNK_180224860,&UNK_1802335d0);
        }
      }
      else if (iVar5 < 3) {
        if (iVar5 == 1) {
          piVar9 = piVar9 + 1;
          if ((char)*piVar9 != '\0') {
            lVar10 = -1;
            do {
              lVar10 = lVar10 + 1;
            } while (*(char *)((longlong)piVar9 + lVar10) != '\0');
          }
          FUN_180006300(auStack_80,piVar9,lVar10);
LAB_18012eb4c:
          if (iStack_178 == 1) {
            puVar8 = auStack_80;
            if (0xf < uStack_68) {
              puVar8 = (undefined8 *)CONCAT71(auStack_80[0]._1_7_,(undefined1)auStack_80[0]);
            }
            iVar6 = atoi(puVar8);
            param_5[0x27] = iVar6;
          }
          else if (iStack_178 == 2) {
            iStack_174 = 0;
            puVar8 = auStack_80;
            if (0xf < uStack_68) {
              puVar8 = (undefined8 *)CONCAT71(auStack_80[0]._1_7_,(undefined1)auStack_80[0]);
            }
            iVar5 = FUN_18012d0c0(0,puVar8,&iStack_174);
            if (iVar5 == 0) {
              param_5[0x28] = iStack_174;
            }
            else {
              cVar4 = FUN_18001dbc0(4);
              if (cVar4 != '\0') {
                if (0xf < (ulonglong)param_4[3]) {
                  param_4 = (longlong *)*param_4;
                }
                uVar7 = FUN_18001db50();
                plStack_1a8 = (longlong *)CONCAT44(plStack_1a8._4_4_,0x1086);
                plStack_198 = param_4;
LAB_18012ec60:
                uStack_1a0 = (longlong *)CONCAT44(uStack_1a0._4_4_,iVar6);
                FUN_180015b40(uVar7,4,&UNK_1802336a0,&UNK_1802335d0);
              }
            }
          }
          else if (iStack_178 == 7) {
            uStack_108 = 0;
            puVar8 = auStack_80;
            if (0xf < uStack_68) {
              puVar8 = (undefined8 *)CONCAT71(auStack_80[0]._1_7_,(undefined1)auStack_80[0]);
            }
            iVar5 = FUN_18012d190(0,puVar8,&uStack_108);
            if (iVar5 == 0) {
              *(undefined8 *)(param_5 + 0x2a) = uStack_108;
            }
            else {
              cVar4 = FUN_18001dbc0(4);
              if (cVar4 != '\0') {
                if (0xf < (ulonglong)param_4[3]) {
                  param_4 = (longlong *)*param_4;
                }
                uVar7 = FUN_18001db50();
                plStack_1a8 = (longlong *)CONCAT44(plStack_1a8._4_4_,0x1093);
                plStack_198 = param_4;
                goto LAB_18012ec60;
              }
            }
          }
          else if (iStack_178 == 3) {
            if (param_5 + 0x2c != (int *)auStack_80) {
              FUN_18012fac0(param_5 + 0x2c,auStack_80);
              uVar7 = *(undefined8 *)(param_5 + 0x30);
              *(undefined8 *)(param_5 + 0x30) = uStack_70;
              uVar3 = *(ulonglong *)(param_5 + 0x32);
              *(ulonglong *)(param_5 + 0x32) = uStack_68;
              uStack_70 = uVar7;
              uStack_68 = uVar3;
            }
          }
          else {
            cVar4 = FUN_18001dbc0(4);
            if (cVar4 != '\0') {
              if (0xf < (ulonglong)param_4[3]) {
                param_4 = (longlong *)*param_4;
              }
              uVar7 = FUN_18001db50();
              uStack_1a0 = (longlong *)CONCAT44(uStack_1a0._4_4_,iVar6);
              plStack_1a8 = (longlong *)CONCAT44(plStack_1a8._4_4_,0x109c);
              plStack_198 = param_4;
              FUN_180015b40(uVar7,4,&UNK_180233700,&UNK_1802335d0);
            }
          }
        }
        else {
          uVar7 = FUN_18008e5d0();
          FUN_18010a690(uVar7,&lStack_158,iVar6);
          if (lStack_158 == 0) {
            cVar4 = FUN_18001dbc0(4);
            if (cVar4 != '\0') {
              if (0xf < (ulonglong)param_4[3]) {
                param_4 = (longlong *)*param_4;
              }
              uVar7 = FUN_18001db50();
              uStack_1a0 = (longlong *)CONCAT44(uStack_1a0._4_4_,iVar6);
              plStack_1a8 = (longlong *)CONCAT44(plStack_1a8._4_4_,0x1067);
              plStack_198 = param_4;
              FUN_180015b40(uVar7,4,&UNK_1802319c0,&UNK_1802335d0);
            }
          }
          else {
            uVar12 = *(undefined4 *)(lStack_158 + 0x28);
            uVar7 = FUN_18008e5d0();
            FUN_180108ba0(uVar7,&lStack_128,uVar12);
            if (lStack_128 != 0) {
              puVar8 = (undefined8 *)FUN_18001ec90(lStack_128,&uStack_60);
              if (auStack_80 != puVar8) {
                if (0xf < uStack_68) {
                  __3_YAXPEAX_Z(CONCAT71(auStack_80[0]._1_7_,(undefined1)auStack_80[0]));
                }
                uStack_68 = 0xf;
                uStack_70 = 0;
                auStack_80[0]._0_1_ = 0;
                FUN_18001f540(auStack_80,puVar8);
              }
              if (0xf < uStack_48) {
                __3_YAXPEAX_Z(CONCAT71(uStack_5f,uStack_60));
              }
              plVar11 = plStack_120;
              uStack_48 = 0xf;
              uStack_50 = 0;
              uStack_60 = 0;
              if (plStack_120 != (longlong *)0x0) {
                LOCK();
                plVar1 = plStack_120 + 1;
                *(int *)plVar1 = (int)*plVar1 + -1;
                UNLOCK();
                if ((int)*plVar1 == 0) {
                  (**(code **)*plStack_120)(plStack_120);
                  LOCK();
                  piVar9 = (int *)((longlong)plVar11 + 0xc);
                  *piVar9 = *piVar9 + -1;
                  UNLOCK();
                  if (*piVar9 == 0) {
                    (**(code **)(*plStack_120 + 8))();
                  }
                }
              }
              plVar11 = plStack_150;
              if (plStack_150 != (longlong *)0x0) {
                LOCK();
                plVar1 = plStack_150 + 1;
                *(int *)plVar1 = (int)*plVar1 + -1;
                UNLOCK();
                if ((int)*plVar1 == 0) {
                  (**(code **)*plStack_150)(plStack_150);
                  LOCK();
                  piVar9 = (int *)((longlong)plVar11 + 0xc);
                  *piVar9 = *piVar9 + -1;
                  UNLOCK();
                  if (*piVar9 == 0) {
                    (**(code **)(*plStack_150 + 8))();
                  }
                }
              }
              goto LAB_18012eb4c;
            }
            cVar4 = FUN_18001dbc0(4);
            if (cVar4 != '\0') {
              if (0xf < (ulonglong)param_4[3]) {
                param_4 = (longlong *)*param_4;
              }
              uVar7 = FUN_18001db50();
              uStack_1a0 = (longlong *)CONCAT44(uStack_1a0._4_4_,uVar12);
              plStack_1a8 = (longlong *)CONCAT44(plStack_1a8._4_4_,0x1070);
              plStack_198 = param_4;
              FUN_180015b40(uVar7,4,&UNK_1802318c0,&UNK_1802335d0);
            }
            plVar11 = plStack_120;
            if (plStack_120 != (longlong *)0x0) {
              LOCK();
              plVar1 = plStack_120 + 1;
              *(int *)plVar1 = (int)*plVar1 + -1;
              UNLOCK();
              if ((int)*plVar1 == 0) {
                (**(code **)*plStack_120)(plStack_120);
                LOCK();
                piVar9 = (int *)((longlong)plVar11 + 0xc);
                *piVar9 = *piVar9 + -1;
                UNLOCK();
                if (*piVar9 == 0) {
                  (**(code **)(*plStack_120 + 8))();
                }
              }
            }
          }
          plVar11 = plStack_150;
          if (plStack_150 != (longlong *)0x0) {
            LOCK();
            plVar1 = plStack_150 + 1;
            *(int *)plVar1 = (int)*plVar1 + -1;
            UNLOCK();
            if ((int)*plVar1 == 0) {
              (**(code **)*plStack_150)(plStack_150);
              LOCK();
              piVar9 = (int *)((longlong)plVar11 + 0xc);
              *piVar9 = *piVar9 + -1;
              iVar6 = *piVar9;
              UNLOCK();
              goto joined_r0x00018012e9c3;
            }
          }
        }
      }
      else {
        if (iVar5 != 3) goto LAB_18012ed1a;
        iVar5 = piVar9[0x23];
        piVar2 = piVar9 + 0x24;
        if ((char)*piVar2 != '\0') {
          lVar10 = -1;
          do {
            lVar10 = lVar10 + 1;
          } while (*(char *)((longlong)piVar2 + lVar10) != '\0');
        }
        plVar11 = (longlong *)(param_5 + 0x34);
        iStack_160 = iVar5;
        FUN_180006300(plVar11,piVar2,lVar10);
        uStack_138 = 0;
        plStack_130 = (longlong *)0x0;
        cVar4 = FUN_180125040(0,iVar5);
        if (cVar4 == '\0') {
          uVar7 = FUN_18008e6c0(uStack_100);
          uVar7 = FUN_1800cae00(uVar7,auStack_c8,iVar5);
          uVar12 = FUN_18001d870(&uStack_138,uVar7);
          if (plStack_c0 != (longlong *)0x0) {
            LOCK();
            plVar1 = plStack_c0 + 1;
            *(int *)plVar1 = (int)*plVar1 + -1;
            UNLOCK();
            piVar9 = piStack_170;
            if ((int)*plVar1 == 0) {
              uVar12 = (**(code **)*plStack_c0)(plStack_c0);
              LOCK();
              piVar2 = (int *)((longlong)plStack_c0 + 0xc);
              *piVar2 = *piVar2 + -1;
              UNLOCK();
              piVar9 = piStack_170;
              if (*piVar2 == 0) {
                uVar12 = (**(code **)(*plStack_c0 + 8))(plStack_c0);
                piVar9 = piStack_170;
              }
            }
          }
        }
        else {
          uStack_a8 = 0x1802b1208;
          FUN_1801e52d0();
          uVar7 = FUN_18008e910();
          uVar7 = FUN_1800cae00(uVar7,auStack_d8,iVar5);
          FUN_18001d870(&uStack_138,uVar7);
          if (plStack_d0 != (longlong *)0x0) {
            LOCK();
            plVar1 = plStack_d0 + 1;
            *(int *)plVar1 = (int)*plVar1 + -1;
            UNLOCK();
            piVar9 = piStack_170;
            if ((int)*plVar1 == 0) {
              (**(code **)*plStack_d0)(plStack_d0);
              LOCK();
              piVar2 = (int *)((longlong)plStack_d0 + 0xc);
              *piVar2 = *piVar2 + -1;
              UNLOCK();
              piVar9 = piStack_170;
              if (*piVar2 == 0) {
                (**(code **)(*plStack_d0 + 8))(plStack_d0);
                piVar9 = piStack_170;
              }
            }
          }
          uVar12 = FUN_1801e5390(0x1802b1208);
        }
        plStack_1a8 = plVar11;
        iVar5 = FUN_18012dcb0(uVar12,iStack_178,plVar11,&uStack_138);
        if (iVar5 == 0) {
          uVar7 = FUN_18008e5d0();
          iVar5 = iStack_160;
          FUN_18010a720(uVar7,&lStack_118,iStack_160);
          if (lStack_118 == 0) {
            cVar4 = FUN_18001dbc0(4);
            if (cVar4 != '\0') {
              uVar7 = FUN_18001db50();
              plStack_198 = (longlong *)CONCAT44(plStack_198._4_4_,iVar6);
              uStack_1a0 = (longlong *)CONCAT44(uStack_1a0._4_4_,iVar5);
              plStack_1a8 = (longlong *)CONCAT44(plStack_1a8._4_4_,0x10c5);
              FUN_180015b40(uVar7,4,&UNK_180233668,&UNK_1802335d0);
            }
          }
          else {
            if (plStack_110 != (longlong *)0x0) {
              LOCK();
              *(int *)(plStack_110 + 1) = (int)plStack_110[1] + 1;
              UNLOCK();
              piVar9 = piStack_170;
            }
            plVar11 = *(longlong **)(param_5 + 0x5a);
            *(longlong **)(param_5 + 0x5a) = plStack_110;
            *(longlong *)(param_5 + 0x58) = lStack_118;
            plStack_e0 = plVar11;
            if (plVar11 != (longlong *)0x0) {
              LOCK();
              plVar1 = plVar11 + 1;
              *(int *)plVar1 = (int)*plVar1 + -1;
              UNLOCK();
              piVar9 = piStack_170;
              if ((int)*plVar1 == 0) {
                (**(code **)*plVar11)(plVar11);
                LOCK();
                piVar2 = (int *)((longlong)plVar11 + 0xc);
                *piVar2 = *piVar2 + -1;
                UNLOCK();
                piVar9 = piStack_170;
                if (*piVar2 == 0) {
                  (**(code **)(*plStack_e0 + 8))();
                  piVar9 = piStack_170;
                }
              }
            }
          }
          iVar6 = piVar9[1];
          param_5[0x5c] = iVar6;
          if ((iVar6 == 0) || (iVar6 == 1)) {
            param_5[0x5d] = 0;
          }
          else if (iVar6 == 2) {
            param_5[0x5d] = piVar9[2];
          }
          else if (iVar6 == 3) {
            iVar6 = piVar9[2];
            piVar9 = piVar9 + 3;
            if ((char)*piVar9 == '\0') {
              lVar10 = 0;
            }
            else {
              lVar10 = -1;
              do {
                lVar10 = lVar10 + 1;
              } while (*(char *)((longlong)piVar9 + lVar10) != '\0');
            }
            plVar11 = (longlong *)(param_5 + 0x5e);
            FUN_180006300(plVar11,piVar9,lVar10);
            uStack_148 = 0;
            plStack_140 = (longlong *)0x0;
            cVar4 = FUN_180125040(0,iVar6);
            if (cVar4 == '\0') {
              uVar7 = FUN_18008e6c0(uStack_100);
              uVar7 = FUN_1800cae00(uVar7,auStack_a0,iVar6);
              uVar12 = FUN_18001d870(&uStack_148,uVar7);
              if (plStack_98 != (longlong *)0x0) {
                LOCK();
                plVar1 = plStack_98 + 1;
                *(int *)plVar1 = (int)*plVar1 + -1;
                UNLOCK();
                if ((int)*plVar1 == 0) {
                  uVar12 = (**(code **)*plStack_98)(plStack_98);
                  LOCK();
                  piVar9 = (int *)((longlong)plStack_98 + 0xc);
                  *piVar9 = *piVar9 + -1;
                  UNLOCK();
                  if (*piVar9 == 0) {
                    uVar12 = (**(code **)(*plStack_98 + 8))(plStack_98);
                  }
                }
              }
            }
            else {
              uStack_90 = 0x1802b1208;
              FUN_1801e52d0(0x1802b1208);
              uVar7 = FUN_18008e910();
              uVar7 = FUN_1800cae00(uVar7,auStack_b8,iVar6);
              FUN_18001d870(&uStack_148,uVar7);
              if (plStack_b0 != (longlong *)0x0) {
                LOCK();
                plVar1 = plStack_b0 + 1;
                *(int *)plVar1 = (int)*plVar1 + -1;
                UNLOCK();
                if ((int)*plVar1 == 0) {
                  (**(code **)*plStack_b0)(plStack_b0);
                  LOCK();
                  piVar9 = (int *)((longlong)plStack_b0 + 0xc);
                  *piVar9 = *piVar9 + -1;
                  UNLOCK();
                  if (*piVar9 == 0) {
                    (**(code **)(*plStack_b0 + 8))(plStack_b0);
                  }
                }
              }
              uVar12 = FUN_1801e5390(0x1802b1208);
            }
            plStack_1a8 = plVar11;
            iVar5 = FUN_18012dcb0(uVar12,1,plVar11,&uStack_148);
            if ((iVar5 != 0) && (cVar4 = FUN_18001dbc0(4), cVar4 != '\0')) {
              if (0xf < *(ulonglong *)(param_5 + 100)) {
                plVar11 = (longlong *)*plVar11;
              }
              uVar7 = FUN_18001db50();
              uStack_1a0 = (longlong *)CONCAT44(uStack_1a0._4_4_,iVar6);
              plStack_1a8 = (longlong *)CONCAT44(plStack_1a8._4_4_,0x10fc);
              plStack_198 = plVar11;
              FUN_180015b40(uVar7,4,&UNK_1802337b0,&UNK_1802335d0);
            }
            plVar11 = plStack_140;
            if (plStack_140 != (longlong *)0x0) {
              LOCK();
              plVar1 = plStack_140 + 1;
              *(int *)plVar1 = (int)*plVar1 + -1;
              UNLOCK();
              if ((int)*plVar1 == 0) {
                (**(code **)*plStack_140)(plStack_140);
                LOCK();
                piVar9 = (int *)((longlong)plVar11 + 0xc);
                *piVar9 = *piVar9 + -1;
                UNLOCK();
                if (*piVar9 == 0) {
                  (**(code **)(*plStack_140 + 8))();
                }
              }
            }
          }
          else {
            cVar4 = FUN_18001dbc0(4);
            if (cVar4 != '\0') {
              uVar7 = FUN_18001db50();
              plStack_1a8 = (longlong *)CONCAT44(plStack_1a8._4_4_,0x1104);
              FUN_180015b40(uVar7,4,&UNK_18021c668,&UNK_1802335d0);
            }
          }
          plVar11 = plStack_110;
          if (plStack_110 != (longlong *)0x0) {
            LOCK();
            plVar1 = plStack_110 + 1;
            *(int *)plVar1 = (int)*plVar1 + -1;
            UNLOCK();
            if ((int)*plVar1 == 0) {
              (**(code **)*plStack_110)(plStack_110);
              LOCK();
              piVar9 = (int *)((longlong)plVar11 + 0xc);
              *piVar9 = *piVar9 + -1;
              UNLOCK();
              if (*piVar9 == 0) {
                (**(code **)(*plStack_110 + 8))();
              }
            }
          }
        }
        else {
          cVar4 = FUN_18001dbc0(4);
          if (cVar4 != '\0') {
            if (0xf < *(ulonglong *)(param_5 + 0x3a)) {
              plVar11 = (longlong *)*plVar11;
            }
            if (0xf < (ulonglong)param_4[3]) {
              param_4 = (longlong *)*param_4;
            }
            uVar7 = FUN_18001db50();
            iStack_190 = iStack_160;
            uStack_1a0 = (longlong *)CONCAT44(uStack_1a0._4_4_,iVar6);
            plStack_1a8 = (longlong *)CONCAT44(plStack_1a8._4_4_,0x10bd);
            plStack_198 = param_4;
            lStack_188 = (longlong)plVar11;
            FUN_180015b40(uVar7,4,&UNK_180233610,&UNK_1802335d0);
          }
        }
        plVar11 = plStack_130;
        if (plStack_130 != (longlong *)0x0) {
          LOCK();
          plVar1 = plStack_130 + 1;
          *(int *)plVar1 = (int)*plVar1 + -1;
          UNLOCK();
          if ((int)*plVar1 == 0) {
            (**(code **)*plStack_130)(plStack_130);
            LOCK();
            piVar9 = (int *)((longlong)plVar11 + 0xc);
            *piVar9 = *piVar9 + -1;
            iVar6 = *piVar9;
            UNLOCK();
            plStack_150 = plStack_130;
joined_r0x00018012e9c3:
            if (iVar6 == 0) {
              (**(code **)(*plStack_150 + 8))();
            }
          }
        }
      }
      if (0xf < uStack_68) {
        __3_YAXPEAX_Z(CONCAT71(auStack_80[0]._1_7_,(undefined1)auStack_80[0]));
      }
      uStack_68 = 0xf;
      uStack_70 = 0;
      auStack_80[0]._0_1_ = 0;
    }
    else {
      cVar4 = FUN_18001dbc0(4);
      if (cVar4 != '\0') {
        uVar7 = FUN_18001db50();
        uStack_1a0 = (longlong *)CONCAT44(uStack_1a0._4_4_,iVar6);
        plStack_1a8 = (longlong *)CONCAT44(plStack_1a8._4_4_,0x1050);
        FUN_180015b40(uVar7,4,&UNK_180233570,&UNK_1802335d0);
      }
    }
    plVar11 = plStack_f0;
    if (plStack_f0 != (longlong *)0x0) {
      LOCK();
      plVar1 = plStack_f0 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plStack_f0)(plStack_f0);
        LOCK();
        piVar9 = (int *)((longlong)plVar11 + 0xc);
        *piVar9 = *piVar9 + -1;
        UNLOCK();
        if (*piVar9 == 0) {
          (**(code **)(*plStack_f0 + 8))();
        }
      }
    }
  }
  plVar11 = plStack_168;
  if (plStack_168 != (longlong *)0x0) {
    LOCK();
    plVar1 = plStack_168 + 1;
    *(int *)plVar1 = (int)*plVar1 + -1;
    UNLOCK();
    if ((int)*plVar1 == 0) {
      (**(code **)*plStack_168)(plStack_168);
      LOCK();
      piVar9 = (int *)((longlong)plVar11 + 0xc);
      *piVar9 = *piVar9 + -1;
      UNLOCK();
      if (*piVar9 == 0) {
        (**(code **)(*plStack_168 + 8))();
      }
    }
  }
LAB_18012ede7:
  FUN_1801e8810(uStack_40 ^ (ulonglong)auStack_1c8);
  return;
}

