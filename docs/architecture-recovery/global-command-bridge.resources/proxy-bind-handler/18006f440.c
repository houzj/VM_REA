
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_18006f440(undefined8 param_1,uint *param_2,longlong *param_3,int *param_4)

{
  uint uVar1;
  int iVar2;
  undefined1 auVar3 [16];
  char cVar4;
  undefined4 uVar5;
  int iVar6;
  longlong *plVar7;
  undefined8 uVar8;
  int *piVar9;
  longlong lVar10;
  undefined4 *puVar11;
  char *pcVar12;
  undefined1 *puVar13;
  ulonglong uVar14;
  uint *puVar15;
  longlong lVar16;
  int iVar17;
  undefined1 auStack_178 [32];
  undefined1 *puStack_158;
  undefined4 uStack_150;
  undefined4 uStack_148;
  undefined4 uStack_144;
  undefined4 auStack_140 [2];
  int *piStack_138;
  undefined4 auStack_130 [2];
  longlong *plStack_128;
  undefined1 auStack_120 [8];
  undefined1 auStack_118 [8];
  undefined1 auStack_110 [8];
  undefined1 auStack_108 [8];
  undefined1 auStack_100 [8];
  undefined1 auStack_f8 [8];
  undefined1 auStack_f0 [8];
  undefined8 uStack_e8;
  undefined1 auStack_e0 [8];
  undefined1 auStack_d8 [8];
  undefined1 auStack_d0 [8];
  undefined1 auStack_c8 [8];
  undefined1 auStack_c0 [8];
  undefined1 auStack_b8 [8];
  undefined1 auStack_b0 [8];
  undefined1 auStack_a8 [8];
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
  
  uStack_e8 = 0xfffffffffffffffe;
  uStack_40 = _DAT_180192230 ^ (ulonglong)auStack_178;
  uStack_148 = 0xe00000ff;
  plStack_128 = param_3;
  if ((param_2 == (uint *)0x0) || (param_4 == (int *)0x0)) {
    cVar4 = FUN_18000c2c0(4);
    if (cVar4 != '\0') {
      uVar8 = FUN_18000bfe0();
      puStack_158 = (undefined1 *)CONCAT44(puStack_158._4_4_,0x2c8e);
      FUN_18000c1c0(uVar8,4,&UNK_180156320,&UNK_1801562f0);
    }
  }
  else {
    uVar14 = *(ulonglong *)(param_2 + 2);
    uVar1 = *param_2;
    plVar7 = (longlong *)FUN_180027e50(param_2,&piStack_138,&UNK_18014f0dc);
    if (*plVar7 == (uVar14 & 0xffffffffffff) + (ulonglong)uVar1 * 0x20) {
      cVar4 = FUN_18000c2c0(4);
      if (cVar4 != '\0') {
        uVar8 = FUN_18000bfe0();
        puStack_158 = (undefined1 *)CONCAT44(puStack_158._4_4_,0x2c96);
        FUN_18000c1c0(uVar8,4,&UNK_180156428,&UNK_1801562f0);
      }
      uStack_148 = 0xe0000109;
      auStack_130[0] = 0xe0000109;
      func_0x00018011e41a(auStack_130,&UNK_18018ea80);
    }
    uVar8 = FUN_180028310(param_2,&UNK_18014f0dc);
    piVar9 = (int *)FUN_180028310(uVar8,&UNK_180156468);
    *param_4 = *piVar9;
    piVar9 = (int *)FUN_180028310(uVar8,&UNK_1801563d8);
    piStack_138 = piVar9;
    if (*(short *)((longlong)piVar9 + 0xe) == 4) {
      iVar2 = *piVar9;
      if (iVar2 != *param_4) {
        cVar4 = FUN_18000c2c0(4);
        if (cVar4 != '\0') {
          uVar8 = FUN_18000bfe0();
          puStack_158 = (undefined1 *)CONCAT44(puStack_158._4_4_,0x2ca4);
          FUN_18000c1c0(uVar8,4,&UNK_1801563e8,&UNK_1801562f0);
        }
        uStack_148 = 0xe0000109;
        uStack_144 = 0xe0000109;
        piVar9 = (int *)func_0x00018011e41a(&uStack_144,&UNK_18018ea80);
      }
      if (0 < iVar2) {
        auVar3._8_8_ = 0;
        auVar3._0_8_ = (longlong)iVar2;
        uVar8 = SUB168(ZEXT816(0x358) * auVar3,0);
        if (SUB168(ZEXT816(0x358) * auVar3,8) != 0) {
          uVar8 = 0xffffffffffffffff;
        }
        lVar10 = func_0x00018011dc64(uVar8,0x180197811);
        *param_3 = lVar10;
        piVar9 = piStack_138;
        if (lVar10 == 0) {
          cVar4 = FUN_18000c2c0(4);
          if (cVar4 != '\0') {
            uVar5 = GetLastError();
            uVar8 = FUN_18000bfe0();
            puStack_158 = (undefined1 *)CONCAT44(puStack_158._4_4_,0x2cae);
            uStack_150 = uVar5;
            FUN_18000c1c0(uVar8,4,&UNK_180156500,&UNK_1801562f0);
          }
          uStack_148 = 0xe0000002;
          auStack_140[0] = 0xe0000002;
          piVar9 = (int *)func_0x00018011e41a(auStack_140,&UNK_18018ea80);
        }
      }
      uVar14 = 0;
      while (iVar17 = (int)uVar14, iVar17 < iVar2) {
        puVar15 = (uint *)(uVar14 * 0x10 + (*(ulonglong *)(piVar9 + 2) & 0xffffffffffff));
        puVar11 = (undefined4 *)FUN_180028310(puVar15,&UNK_180151970);
        lVar10 = (longlong)iVar17 * 0x358;
        *(undefined4 *)(lVar10 + *param_3) = *puVar11;
        piVar9 = (int *)FUN_180028310(puVar15,&UNK_180156548);
        iVar6 = *piVar9;
        if (iVar6 == 1) {
          *(undefined4 *)(lVar10 + 4 + *param_3) = 1;
        }
        else if (iVar6 == 2) {
          *(undefined4 *)(lVar10 + 4 + *param_3) = 2;
        }
        else if (iVar6 == 3) {
          *(undefined4 *)(lVar10 + 4 + *param_3) = 3;
        }
        else if (iVar6 == 4) {
          *(undefined4 *)(lVar10 + 4 + *param_3) = 4;
        }
        else if (iVar6 == 5) {
          *(undefined4 *)(lVar10 + 4 + *param_3) = 5;
        }
        else if (iVar6 == 6) {
          *(undefined4 *)(lVar10 + 4 + *param_3) = 6;
        }
        else if (iVar6 == 7) {
          *(undefined4 *)(lVar10 + 4 + *param_3) = 7;
        }
        else {
          *(undefined4 *)(lVar10 + 4 + *param_3) = 0;
        }
        uStack_48 = 0xf;
        uStack_50 = 0;
        uStack_60 = 0;
        FUN_1800094c0(&uStack_60,&UNK_18014552d,0);
        uVar1 = *puVar15;
        uVar14 = *(ulonglong *)(puVar15 + 2);
        plVar7 = (longlong *)FUN_180027e50(puVar15,auStack_a8,&UNK_180154d68);
        if (*plVar7 != (ulonglong)uVar1 * 0x20 + (uVar14 & 0xffffffffffff)) {
          pcVar12 = (char *)FUN_180028310(puVar15,&UNK_180154d68);
          if ((*(ushort *)(pcVar12 + 0xe) & 0x1000) == 0) {
            pcVar12 = (char *)(*(ulonglong *)(pcVar12 + 8) & 0xffffffffffff);
          }
          if (*pcVar12 == '\0') {
            lVar16 = 0;
          }
          else {
            lVar16 = -1;
            do {
              lVar16 = lVar16 + 1;
            } while (pcVar12[lVar16] != '\0');
          }
          FUN_1800094c0(&uStack_60,pcVar12,lVar16);
        }
        puStack_158 = &uStack_60;
        if (0xf < uStack_48) {
          puStack_158 = (undefined1 *)CONCAT71(uStack_5f,uStack_60);
        }
        _snprintf_s(*param_3 + 8 + lVar10,0x80,0x7f,&UNK_18014647c);
        puVar15 = (uint *)FUN_180028310(puVar15,&UNK_180154cb8);
        uVar1 = *puVar15;
        uVar14 = *(ulonglong *)(puVar15 + 2);
        plVar7 = (longlong *)FUN_180027e50(puVar15,auStack_108,&UNK_180154cc0);
        if (((*plVar7 == (ulonglong)uVar1 * 0x20 + (uVar14 & 0xffffffffffff)) ||
            (uVar1 = *puVar15, uVar14 = *(ulonglong *)(puVar15 + 2),
            plVar7 = (longlong *)FUN_180027e50(puVar15,auStack_c8,&UNK_180154c98),
            *plVar7 != (ulonglong)uVar1 * 0x20 + (uVar14 & 0xffffffffffff))) ||
           (uVar1 = *puVar15, uVar14 = *(ulonglong *)(puVar15 + 2),
           plVar7 = (longlong *)FUN_180027e50(puVar15,auStack_f8,&UNK_180154ca8),
           *plVar7 != (ulonglong)uVar1 * 0x20 + (uVar14 & 0xffffffffffff))) {
          uVar1 = *puVar15;
          uVar14 = *(ulonglong *)(puVar15 + 2);
          plVar7 = (longlong *)FUN_180027e50(puVar15,auStack_b0,&UNK_180154c98);
          if (((*plVar7 == (ulonglong)uVar1 * 0x20 + (uVar14 & 0xffffffffffff)) ||
              (uVar1 = *puVar15, uVar14 = *(ulonglong *)(puVar15 + 2),
              plVar7 = (longlong *)FUN_180027e50(puVar15,auStack_120,&UNK_180154cc0),
              *plVar7 != (ulonglong)uVar1 * 0x20 + (uVar14 & 0xffffffffffff))) ||
             (uVar1 = *puVar15, uVar14 = *(ulonglong *)(puVar15 + 2),
             plVar7 = (longlong *)FUN_180027e50(puVar15,auStack_b8,&UNK_180154ca8),
             *plVar7 != (ulonglong)uVar1 * 0x20 + (uVar14 & 0xffffffffffff))) {
            uVar1 = *puVar15;
            uVar14 = *(ulonglong *)(puVar15 + 2);
            plVar7 = (longlong *)FUN_180027e50(puVar15,auStack_d8);
            if (((*plVar7 == (ulonglong)uVar1 * 0x20 + (uVar14 & 0xffffffffffff)) ||
                (uVar1 = *puVar15, uVar14 = *(ulonglong *)(puVar15 + 2),
                plVar7 = (longlong *)FUN_180027e50(puVar15,auStack_118),
                *plVar7 != (ulonglong)uVar1 * 0x20 + (uVar14 & 0xffffffffffff))) ||
               (uVar1 = *puVar15, uVar14 = *(ulonglong *)(puVar15 + 2),
               plVar7 = (longlong *)FUN_180027e50(puVar15,auStack_110),
               *plVar7 != (ulonglong)uVar1 * 0x20 + (uVar14 & 0xffffffffffff))) {
              *(undefined4 *)(lVar10 + 0x88 + *param_3) = 0;
              cVar4 = FUN_18000c2c0(3);
              if (cVar4 != '\0') {
                uVar8 = FUN_18000bfe0();
                puStack_158 = (undefined1 *)CONCAT44(puStack_158._4_4_,0x2d1f);
                FUN_18000c1c0(uVar8,3,&UNK_1801564b0,&UNK_1801562f0);
              }
              goto LAB_18007007c;
            }
            *(undefined4 *)(lVar10 + 0x88 + *param_3) = 3;
            puVar15 = (uint *)FUN_180028310(puVar15,&UNK_180154ca8);
            puVar11 = (undefined4 *)FUN_180028310(puVar15,&UNK_18014f6f8);
            uVar5 = *puVar11;
            pcVar12 = (char *)FUN_180028310(puVar15,&UNK_180154ce0);
            if ((*(ushort *)(pcVar12 + 0xe) & 0x1000) == 0) {
              pcVar12 = (char *)(*(ulonglong *)(pcVar12 + 8) & 0xffffffffffff);
            }
            uStack_68 = 0xf;
            uStack_70 = 0;
            uStack_80 = 0;
            if (*pcVar12 == '\0') {
              lVar16 = 0;
            }
            else {
              lVar16 = -1;
              do {
                lVar16 = lVar16 + 1;
              } while (pcVar12[lVar16] != '\0');
            }
            FUN_1800094c0(&uStack_80,pcVar12,lVar16);
            *(undefined4 *)(lVar10 + 0x154 + *param_3) = uVar5;
            puStack_158 = &uStack_80;
            if (0xf < uStack_68) {
              puStack_158 = (undefined1 *)CONCAT71(uStack_7f,uStack_80);
            }
            _snprintf_s(*param_3 + 0x158 + lVar10,0x80,0x7f,&UNK_18014647c);
            uVar1 = *puVar15;
            uVar14 = *(ulonglong *)(puVar15 + 2);
            plVar7 = (longlong *)FUN_180027e50(puVar15,auStack_100);
            if (*plVar7 != (ulonglong)uVar1 * 0x20 + (uVar14 & 0xffffffffffff)) {
              puVar15 = (uint *)FUN_180028310(puVar15,&UNK_18014f654);
              uVar1 = *puVar15;
              uVar14 = *(ulonglong *)(puVar15 + 2);
              plVar7 = (longlong *)FUN_180027e50(puVar15,auStack_f0,&UNK_180154cf0);
              if ((*plVar7 == (ulonglong)uVar1 * 0x20 + (uVar14 & 0xffffffffffff)) ||
                 (uVar1 = *puVar15, uVar14 = *(ulonglong *)(puVar15 + 2),
                 plVar7 = (longlong *)FUN_180027e50(puVar15,auStack_e0,&UNK_180154cd0),
                 *plVar7 != (ulonglong)uVar1 * 0x20 + (uVar14 & 0xffffffffffff))) {
                uVar1 = *puVar15;
                uVar14 = *(ulonglong *)(puVar15 + 2);
                plVar7 = (longlong *)FUN_180027e50(puVar15,auStack_d0);
                if ((*plVar7 != (ulonglong)uVar1 * 0x20 + (uVar14 & 0xffffffffffff)) ||
                   (uVar1 = *puVar15, uVar14 = *(ulonglong *)(puVar15 + 2),
                   plVar7 = (longlong *)FUN_180027e50(puVar15,auStack_c0),
                   *plVar7 == (ulonglong)uVar1 * 0x20 + (uVar14 & 0xffffffffffff))) {
                  *(undefined4 *)(lVar10 + 0x8c + *param_3) = 0;
                  cVar4 = FUN_18000c2c0(3);
                  if (cVar4 != '\0') {
                    uVar8 = FUN_18000bfe0();
                    puStack_158 = (undefined1 *)CONCAT44(puStack_158._4_4_,0x2d18);
                    FUN_18000c1c0(uVar8,3,&UNK_180156478,&UNK_1801562f0);
                  }
                  goto LAB_180070022;
                }
                *(undefined4 *)(lVar10 + 0x8c + *param_3) = 3;
                uVar8 = FUN_180028310(puVar15,&UNK_180154cd0);
                puVar11 = (undefined4 *)FUN_180028310(uVar8,&UNK_18014f268);
                uVar5 = *puVar11;
                pcVar12 = (char *)FUN_180028310(uVar8,&UNK_180154d68);
                if ((*(ushort *)(pcVar12 + 0xe) & 0x1000) == 0) {
                  pcVar12 = (char *)(*(ulonglong *)(pcVar12 + 8) & 0xffffffffffff);
                }
                uStack_88 = 0xf;
                lVar16 = 0;
                uStack_90 = 0;
                uStack_a0 = 0;
                if (*pcVar12 != '\0') {
                  lVar16 = -1;
                  do {
                    lVar16 = lVar16 + 1;
                  } while (pcVar12[lVar16] != '\0');
                }
                FUN_1800094c0(&uStack_a0,pcVar12,lVar16);
                *(undefined4 *)(lVar10 + 0x90 + *param_3) = uVar5;
                puStack_158 = &uStack_a0;
                if (0xf < uStack_88) {
                  puStack_158 = (undefined1 *)CONCAT71(uStack_9f,uStack_a0);
                }
                _snprintf_s(*param_3 + 0x94 + lVar10,0x80,0x7f,&UNK_18014647c);
              }
              else {
                pcVar12 = (char *)FUN_180028310(puVar15,&UNK_180154cf0);
                if ((*(ushort *)(pcVar12 + 0xe) & 0x1000) == 0) {
                  pcVar12 = (char *)(*(ulonglong *)(pcVar12 + 8) & 0xffffffffffff);
                }
                uStack_88 = 0xf;
                lVar16 = 0;
                uStack_90 = 0;
                uStack_a0 = 0;
                if (*pcVar12 != '\0') {
                  lVar16 = -1;
                  do {
                    lVar16 = lVar16 + 1;
                  } while (pcVar12[lVar16] != '\0');
                }
                FUN_1800094c0(&uStack_a0,pcVar12,lVar16);
                puVar13 = &uStack_a0;
                if (0xf < uStack_88) {
                  puVar13 = (undefined1 *)CONCAT71(uStack_9f,uStack_a0);
                }
                iVar6 = _stricmp(puVar13,&UNK_180154cdc);
                if (iVar6 == 0) {
                  *(undefined4 *)(lVar10 + 0x8c + *param_3) = 1;
                }
                else {
                  *(undefined4 *)(lVar10 + 0x8c + *param_3) = 2;
                  puVar13 = &uStack_a0;
                  if (0xf < uStack_88) {
                    puVar13 = (undefined1 *)CONCAT71(uStack_9f,uStack_a0);
                  }
                  uVar5 = atoi(puVar13);
                  *(undefined4 *)(lVar10 + 0x90 + *param_3) = uVar5;
                }
              }
              if (0xf < uStack_88) {
                __3_YAXPEAX_Z(CONCAT71(uStack_9f,uStack_a0));
              }
            }
          }
          else {
            *(undefined4 *)(lVar10 + 0x88 + *param_3) = 2;
            uVar8 = FUN_180028310(puVar15,&UNK_180154c98);
            pcVar12 = (char *)FUN_180028310(uVar8,&UNK_180154ce0);
            if ((*(ushort *)(pcVar12 + 0xe) & 0x1000) == 0) {
              pcVar12 = (char *)(*(ulonglong *)(pcVar12 + 8) & 0xffffffffffff);
            }
            uStack_68 = 0xf;
            lVar10 = 0;
            uStack_70 = 0;
            uStack_80 = 0;
            if (*pcVar12 != '\0') {
              lVar10 = -1;
              do {
                lVar10 = lVar10 + 1;
              } while (pcVar12[lVar10] != '\0');
            }
            FUN_1800094c0(&uStack_80,pcVar12,lVar10);
          }
LAB_180070022:
          if (0xf < uStack_68) {
            uVar8 = CONCAT71(uStack_7f,uStack_80);
            goto LAB_180070035;
          }
        }
        else {
          *(undefined4 *)(lVar10 + 0x88 + *param_3) = 1;
          pcVar12 = (char *)FUN_180028310(puVar15,&UNK_180154cc0);
          if ((*(ushort *)(pcVar12 + 0xe) & 0x1000) == 0) {
            pcVar12 = (char *)(*(ulonglong *)(pcVar12 + 8) & 0xffffffffffff);
          }
          uStack_88 = 0xf;
          lVar16 = 0;
          uStack_90 = 0;
          uStack_a0 = 0;
          if (*pcVar12 != '\0') {
            lVar16 = -1;
            do {
              lVar16 = lVar16 + 1;
            } while (pcVar12[lVar16] != '\0');
          }
          FUN_1800094c0(&uStack_a0,pcVar12,lVar16);
          puStack_158 = &uStack_a0;
          if (0xf < uStack_88) {
            puStack_158 = (undefined1 *)CONCAT71(uStack_9f,uStack_a0);
          }
          _snprintf_s(*param_3 + 0x8c + lVar10,0x100,0xff,&UNK_18014647c);
          if (0xf < uStack_88) {
            uVar8 = CONCAT71(uStack_9f,uStack_a0);
LAB_180070035:
            __3_YAXPEAX_Z(uVar8);
          }
        }
LAB_18007007c:
        if (0xf < uStack_48) {
          __3_YAXPEAX_Z(CONCAT71(uStack_5f,uStack_60));
        }
        piVar9 = piStack_138;
        uVar14 = (ulonglong)(iVar17 + 1);
      }
    }
  }
  FUN_18011da90(uStack_40 ^ (ulonglong)auStack_178);
  return;
}

