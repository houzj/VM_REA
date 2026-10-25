
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800b8740(longlong param_1)

{
  int *piVar1;
  byte bVar2;
  undefined4 uVar3;
  double dVar4;
  double dVar5;
  double dVar6;
  undefined8 *puVar7;
  char cVar8;
  int iVar9;
  uint uVar10;
  int iVar11;
  int iVar12;
  undefined8 uVar13;
  longlong *plVar14;
  _IMAGE_RUNTIME_FUNCTION_ENTRY *p_Var15;
  longlong *plVar16;
  undefined4 uVar17;
  undefined4 extraout_XMM0_Da;
  undefined4 extraout_XMM0_Da_00;
  undefined8 **ppuVar18;
  double dVar19;
  float fVar20;
  float fVar21;
  float fVar22;
  undefined1 auStack_1c8 [32];
  undefined8 **ppuStack_1a8;
  double dStack_1a0;
  longlong *plStack_198;
  longlong *plStack_190;
  double dStack_188;
  undefined8 *puStack_180;
  double dStack_178;
  float *pfStack_168;
  longlong *plStack_160;
  undefined8 *puStack_158;
  longlong *plStack_150;
  longlong lStack_148;
  longlong *plStack_140;
  longlong lStack_138;
  longlong lStack_130;
  longlong lStack_128;
  longlong lStack_120;
  longlong alStack_118 [2];
  undefined4 uStack_108;
  undefined1 uStack_104;
  undefined2 uStack_103;
  undefined1 uStack_101;
  undefined8 uStack_100;
  undefined4 uStack_f8;
  undefined1 uStack_f4;
  undefined2 uStack_f3;
  undefined1 uStack_f1;
  undefined8 uStack_f0;
  undefined8 auStack_e8 [3];
  ulonglong uStack_d0;
  undefined8 auStack_c8 [3];
  ulonglong uStack_b0;
  undefined8 auStack_a8 [3];
  ulonglong uStack_90;
  ulonglong uStack_88;
  
  alStack_118[1] = 0xfffffffffffffffe;
  uStack_88 = _DAT_1802a5230 ^ (ulonglong)auStack_1c8;
  lStack_138 = param_1 + 0x138;
  iVar9 = _Mtx_lock();
  if (iVar9 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar9);
  }
  if ((*(char *)(param_1 + 0x82) == '\0') || (*(char *)(param_1 + 0x83) == '\0')) {
    cVar8 = FUN_18001dbc0(1);
    if (cVar8 != '\0') {
      plVar14 = (longlong *)(param_1 + 0xa8);
      if (0xf < *(ulonglong *)(param_1 + 0xc0)) {
        plVar14 = (longlong *)*plVar14;
      }
      plVar16 = (longlong *)(param_1 + 0x88);
      if (0xf < *(ulonglong *)(param_1 + 0xa0)) {
        plVar16 = (longlong *)*plVar16;
      }
      uVar13 = FUN_18001db50();
      dStack_1a0 = (double)CONCAT44(dStack_1a0._4_4_,*(undefined4 *)(param_1 + 0x2c));
      ppuStack_1a8 = (undefined8 **)CONCAT44(ppuStack_1a8._4_4_,0x21);
      plStack_198 = plVar16;
      plStack_190 = plVar14;
      FUN_180015b40(uVar13,1,&UNK_1802298e8,&UNK_180229928);
    }
    if (*(char *)(param_1 + 0x83) == '\0') {
      uVar17 = 6;
    }
    else {
      cVar8 = FUN_18001e5c0(param_1);
      uVar17 = 2;
      if (cVar8 != '\0') {
        uVar17 = 4;
      }
    }
    FUN_18001e4e0(param_1,0);
    *(undefined1 *)(param_1 + 0x81) = 0;
    *(undefined4 *)(param_1 + 0x84) = uVar17;
    goto LAB_1800b906c;
  }
  uVar17 = *(undefined4 *)(param_1 + 0x28);
  if ((_DAT_1802b1888 & 1) == 0) {
    _DAT_1802b1888 = _DAT_1802b1888 | 1;
    FUN_180107fc0();
    FUN_1801e8930(&UNK_180202980);
  }
  FUN_180108ba0(&DAT_1802b1890,&lStack_148,uVar17);
  if ((lStack_148 == 0) || (*(char *)(lStack_148 + 0x14c) == '\0')) {
LAB_1800b883f:
    if (*(char *)(param_1 + 0x130) == '\0') {
      *(undefined1 *)(param_1 + 0x130) = 1;
      cVar8 = FUN_18001dbc0(1);
      if (cVar8 != '\0') {
        plVar14 = (longlong *)(param_1 + 0xa8);
        if (0xf < *(ulonglong *)(param_1 + 0xc0)) {
          plVar14 = (longlong *)*plVar14;
        }
        plVar16 = (longlong *)(param_1 + 0x88);
        if (0xf < *(ulonglong *)(param_1 + 0xa0)) {
          plVar16 = (longlong *)*plVar16;
        }
        bVar2 = *(byte *)(param_1 + 0x78);
        uVar13 = FUN_18001db50();
        dStack_188 = (double)CONCAT44(dStack_188._4_4_,(uint)bVar2);
        dStack_1a0 = (double)CONCAT44(dStack_1a0._4_4_,*(undefined4 *)(param_1 + 0x2c));
        ppuStack_1a8 = (undefined8 **)CONCAT44(ppuStack_1a8._4_4_,0x3d);
        plStack_198 = plVar16;
        plStack_190 = plVar14;
        FUN_180015b40(uVar13,1,&UNK_180229940,&UNK_180229928);
      }
      uVar17 = 3;
      ppuVar18 = (undefined8 **)FUN_18008f820();
      dVar19 = 0.0;
      iVar12 = -0x1fffff01;
      iVar9 = -0x1fffff01;
      uStack_108 = *(undefined4 *)(param_1 + 0x2c);
      uStack_f4 = 0;
      uStack_f3 = 0;
      uStack_f1 = 0;
      uStack_100 = *(undefined8 *)(param_1 + 0xf8);
      uStack_104 = 0;
      uStack_103 = 0;
      uStack_101 = 0;
      uStack_f8 = uStack_108;
      uStack_f0 = uStack_100;
      cVar8 = FUN_18001e5c0(param_1);
      dVar4 = dVar19;
      if (cVar8 == '\0') {
        uVar10 = *(uint *)(param_1 + 0x58) >> 1;
        if (((uVar10 & 1) != 0) && (*(char *)(param_1 + 0x78) != '\0')) goto LAB_1800b8aa3;
        if ((*(uint *)(param_1 + 0x58) >> 2 & 1) == 0) {
          if ((uVar10 & 1) == 0) {
            cVar8 = FUN_18001dbc0(4);
            dVar5 = 0.0;
            if (cVar8 != '\0') {
              uVar13 = FUN_18001db50();
              dStack_1a0 = (double)CONCAT44(dStack_1a0._4_4_,*(undefined4 *)(param_1 + 0x2c));
              ppuStack_1a8 = (undefined8 **)CONCAT44(ppuStack_1a8._4_4_,0x8e);
              FUN_180015b40(uVar13,4,&UNK_180229bd8,&UNK_180229928);
              dVar5 = 0.0;
            }
          }
          else {
            VM_M_SetInt(&uStack_108,&UNK_180227678,0,0);
            uVar17 = 1;
            cVar8 = FUN_18001dbc0();
            dVar5 = 0.0;
            if (cVar8 != '\0') {
              uVar13 = FUN_18001db50();
              dStack_1a0 = (double)CONCAT44(dStack_1a0._4_4_,*(undefined4 *)(param_1 + 0x2c));
              ppuStack_1a8 = (undefined8 **)CONCAT44(ppuStack_1a8._4_4_,0x89);
              FUN_180015b40(uVar13,1,&UNK_180229b00,&UNK_180229928);
              dVar5 = 0.0;
            }
          }
        }
        else {
          uVar17 = 2;
          cVar8 = FUN_18001dbc0();
          dVar5 = 0.0;
          if (cVar8 != '\0') {
            uVar13 = FUN_18001db50();
            dStack_1a0 = (double)CONCAT44(dStack_1a0._4_4_,*(undefined4 *)(param_1 + 0x2c));
            ppuStack_1a8 = (undefined8 **)CONCAT44(ppuStack_1a8._4_4_,0x80);
            FUN_180015b40(uVar13,1,&UNK_180229b38,&UNK_180229928);
            dVar5 = 0.0;
          }
        }
      }
      else {
LAB_1800b8aa3:
        iVar11 = QueryPerformanceFrequency(&lStack_128);
        dVar6 = _DAT_18023a7b0;
        dVar5 = 0.0;
        if ((iVar11 != 0) &&
           (iVar11 = QueryPerformanceCounter(&lStack_130), dVar5 = 0.0, iVar11 != 0)) {
          dVar5 = ((double)lStack_130 / (double)lStack_128) * dVar6;
        }
        if (DAT_1802b129e == '\0') {
          plVar14 = *(longlong **)(param_1 + 0x128);
          if (plVar14 == (longlong *)0x0) {
            cVar8 = FUN_18001dbc0(4);
            if (cVar8 != '\0') {
              uVar13 = FUN_18001db50();
              ppuStack_1a8 = (undefined8 **)CONCAT44(ppuStack_1a8._4_4_,0x53);
              FUN_180015b40(uVar13,4,&UNK_180229a28,&UNK_180229928);
            }
            goto LAB_1800b8ddc;
          }
          iVar9 = (**(code **)(*plVar14 + 8))(plVar14,&uStack_f8,&uStack_108);
        }
        else {
          puStack_158 = (undefined8 *)0x0;
          plStack_150 = (longlong *)0x0;
          if ((_DAT_1802b3158 & 1) == 0) {
            _DAT_1802b3158 = _DAT_1802b3158 | 1;
            FUN_1800e8550();
            FUN_1801e8930(&UNK_180202f40);
          }
          ppuStack_1a8 = &puStack_158;
          iVar11 = FUN_1800fdcf0(0x1802b31d8,*(undefined4 *)(param_1 + 0x2c),
                                 *(undefined8 *)(param_1 + 0xf8),param_1 + 0x88);
          puVar7 = puStack_158;
          if (iVar11 == 0) {
            iVar9 = (**(code **)(*(longlong *)*puStack_158 + 8))
                              ((longlong *)*puStack_158,&uStack_f8,&uStack_108);
            *(undefined1 *)(puVar7 + 10) = 1;
          }
          else {
            cVar8 = FUN_18001dbc0(4);
            iVar9 = iVar12;
            if (cVar8 != '\0') {
              plVar14 = (longlong *)FUN_18008f750(auStack_e8,iVar11);
              if (0xf < (ulonglong)plVar14[3]) {
                plVar14 = (longlong *)*plVar14;
              }
              uVar13 = FUN_18001db50();
              dStack_1a0 = (double)CONCAT44(dStack_1a0._4_4_,*(undefined4 *)(param_1 + 0x2c));
              ppuStack_1a8 = (undefined8 **)CONCAT44(ppuStack_1a8._4_4_,100);
              plStack_198 = plVar14;
              FUN_180015b40(uVar13,4,&UNK_1802299e0,&UNK_180229928);
              if (0xf < uStack_d0) {
                __3_YAXPEAX_Z(auStack_e8[0]);
              }
            }
          }
          plVar14 = plStack_150;
          if (plStack_150 != (longlong *)0x0) {
            LOCK();
            plVar16 = plStack_150 + 1;
            *(int *)plVar16 = (int)*plVar16 + -1;
            UNLOCK();
            if ((int)*plVar16 == 0) {
              (**(code **)*plStack_150)(plStack_150);
              LOCK();
              piVar1 = (int *)((longlong)plVar14 + 0xc);
              *piVar1 = *piVar1 + -1;
              UNLOCK();
              if (*piVar1 == 0) {
                (**(code **)(*plStack_150 + 8))();
              }
            }
          }
        }
        if (iVar9 == 0) {
          uVar17 = 0;
        }
        else {
          cVar8 = FUN_18001dbc0(4);
          if (cVar8 != '\0') {
            uVar13 = FUN_18001db50();
            plStack_198 = (longlong *)CONCAT44(plStack_198._4_4_,iVar9);
            dStack_1a0 = (double)CONCAT44(dStack_1a0._4_4_,*(undefined4 *)(param_1 + 0x2c));
            ppuStack_1a8 = (undefined8 **)CONCAT44(ppuStack_1a8._4_4_,0x6e);
            FUN_180015b40(uVar13,4,&UNK_180229ac0,&UNK_180229928);
          }
          VM_M_SetInt(&uStack_108,&UNK_180227678,0,0);
        }
        iVar12 = QueryPerformanceFrequency(alStack_118);
        dVar4 = 0.0;
        if ((iVar12 != 0) &&
           (iVar12 = QueryPerformanceCounter(&lStack_120), dVar4 = dVar19, iVar12 != 0)) {
          dVar4 = ((double)lStack_120 / (double)alStack_118[0]) * dVar6;
        }
        if ((_DAT_1802b1888 & 1) == 0) {
          _DAT_1802b1888 = _DAT_1802b1888 | 1;
          FUN_180107fc0();
          FUN_1801e8930(&UNK_180202980);
        }
        if (*(int *)(param_1 + 0x28) == 100000) {
          _DAT_1802b19e0 = _DAT_1802b19e0 & iVar9 == 0;
        }
        else {
          p_Var15 = (_IMAGE_RUNTIME_FUNCTION_ENTRY *)
                    ((longlong)&_IMAGE_RUNTIME_FUNCTION_ENTRY_1802bc5bc +
                    (longlong)*(int *)(param_1 + 0x28) * -4);
          p_Var15->BeginAddress = p_Var15->BeginAddress & (uint)(iVar9 == 0);
        }
        cVar8 = FUN_18001dbc0(0);
        if (cVar8 != '\0') {
          puStack_180 = (undefined8 *)FUN_18001ead0(extraout_XMM0_Da,auStack_c8,uVar17);
          if (0xf < (ulonglong)puStack_180[3]) {
            puStack_180 = (undefined8 *)*puStack_180;
          }
          plVar14 = (longlong *)(param_1 + 0x88);
          if (0xf < *(ulonglong *)(param_1 + 0xa0)) {
            plVar14 = (longlong *)*plVar14;
          }
          uVar13 = FUN_18001db50();
          dStack_188 = (double)CONCAT44(dStack_188._4_4_,uVar17);
          plStack_190 = (longlong *)CONCAT44(plStack_190._4_4_,iVar9);
          dStack_1a0 = (double)CONCAT44(dStack_1a0._4_4_,*(undefined4 *)(param_1 + 0x2c));
          ppuStack_1a8 = (undefined8 **)CONCAT44(ppuStack_1a8._4_4_,0x78);
          plStack_198 = plVar14;
          FUN_180015b40(uVar13,0,&UNK_180229a50,&UNK_180229928);
          if (0xf < uStack_b0) {
            __3_YAXPEAX_Z(auStack_c8[0]);
          }
        }
      }
LAB_1800b8ddc:
      *(int *)(param_1 + 0x7c) = iVar9;
      *(undefined1 *)(param_1 + 0x130) = 0;
      *(undefined4 *)(param_1 + 0x84) = uVar17;
      dVar19 = (double)FUN_18008f820();
      fVar21 = (float)(dVar19 - (double)ppuVar18);
      fVar22 = (float)(dVar4 - dVar5);
      VM_M_SetFloat(&uStack_108,&UNK_180227668,0,fVar21);
      if (iVar9 == 0) {
        uVar13 = FUN_18008eca0();
        FUN_180093b20(uVar13,*(undefined4 *)(param_1 + 0x2c));
      }
      uVar13 = FUN_18008e6c0(*(undefined8 *)(param_1 + 0xf8));
      ppuStack_1a8 = ppuVar18;
      dStack_1a0 = dVar19;
      FUN_1800c9ea0(uVar13,*(undefined4 *)(param_1 + 0x2c),fVar21,fVar22);
      uVar13 = FUN_18008ed20();
      FUN_1800834b0(uVar13,*(undefined4 *)(param_1 + 0x2c),fVar21,fVar22);
      fVar20 = 0.0;
      uVar3 = *(undefined4 *)(param_1 + 0x2c);
      uVar13 = FUN_18008e6c0(*(undefined8 *)(param_1 + 0xf8));
      FUN_1800cae70(uVar13,&pfStack_168,uVar3);
      plVar14 = plStack_160;
      if (pfStack_168 != (float *)0x0) {
        fVar20 = *pfStack_168;
      }
      if (plStack_160 != (longlong *)0x0) {
        LOCK();
        plVar16 = plStack_160 + 1;
        *(int *)plVar16 = (int)*plVar16 + -1;
        UNLOCK();
        if ((int)*plVar16 == 0) {
          (**(code **)*plStack_160)(plStack_160);
          LOCK();
          piVar1 = (int *)((longlong)plVar14 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (*piVar1 == 0) {
            (**(code **)(*plStack_160 + 8))();
          }
        }
      }
      FUN_18001e4e0(param_1,0);
      *(undefined1 *)(param_1 + 0x81) = 0;
      cVar8 = FUN_18001dbc0(2);
      if (cVar8 != '\0') {
        plVar14 = (longlong *)FUN_18001ead0(extraout_XMM0_Da_00,auStack_a8,uVar17);
        if (0xf < (ulonglong)plVar14[3]) {
          plVar14 = (longlong *)*plVar14;
        }
        uVar13 = FUN_18001db50();
        plStack_198 = (longlong *)CONCAT44(plStack_198._4_4_,uVar17);
        dStack_1a0 = (double)CONCAT44(dStack_1a0._4_4_,*(undefined4 *)(param_1 + 0x2c));
        ppuStack_1a8 = (undefined8 **)CONCAT44(ppuStack_1a8._4_4_,0xae);
        plStack_190 = plVar14;
        dStack_188 = (double)fVar21;
        puStack_180 = (undefined8 *)(double)fVar22;
        dStack_178 = (double)fVar20;
        FUN_180015b40(uVar13,2,&UNK_180229b70,&UNK_180229928);
        if (0xf < uStack_90) {
          __3_YAXPEAX_Z(auStack_a8[0]);
        }
      }
    }
    else {
      cVar8 = FUN_18001dbc0(4);
      if (cVar8 != '\0') {
        plVar14 = (longlong *)(param_1 + 0xa8);
        if (0xf < *(ulonglong *)(param_1 + 0xc0)) {
          plVar14 = (longlong *)*plVar14;
        }
        plVar16 = (longlong *)(param_1 + 0x88);
        if (0xf < *(ulonglong *)(param_1 + 0xa0)) {
          plVar16 = (longlong *)*plVar16;
        }
        uVar13 = FUN_18001db50();
        dStack_1a0 = (double)CONCAT44(dStack_1a0._4_4_,*(undefined4 *)(param_1 + 0x2c));
        ppuStack_1a8 = (undefined8 **)CONCAT44(ppuStack_1a8._4_4_,0x37);
        plStack_198 = plVar16;
        plStack_190 = plVar14;
        FUN_180015b40(uVar13,4,&UNK_180229990,&UNK_180229928);
      }
    }
  }
  else {
    plVar14 = (longlong *)(param_1 + 0x88);
    if (0xf < *(ulonglong *)(param_1 + 0xa0)) {
      plVar14 = (longlong *)*plVar14;
    }
    cVar8 = FUN_18008fb90(plVar14,0);
    if (cVar8 == '\0') goto LAB_1800b883f;
  }
  plVar14 = plStack_140;
  if (plStack_140 != (longlong *)0x0) {
    LOCK();
    plVar16 = plStack_140 + 1;
    *(int *)plVar16 = (int)*plVar16 + -1;
    UNLOCK();
    if ((int)*plVar16 == 0) {
      (**(code **)*plStack_140)(plStack_140);
      LOCK();
      piVar1 = (int *)((longlong)plVar14 + 0xc);
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (*piVar1 == 0) {
        (**(code **)(*plStack_140 + 8))();
      }
    }
  }
LAB_1800b906c:
  iVar9 = _Mtx_unlock(lStack_138);
  if (iVar9 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar9);
  }
  FUN_1801e8810(uStack_88 ^ (ulonglong)auStack_1c8);
  return;
}

