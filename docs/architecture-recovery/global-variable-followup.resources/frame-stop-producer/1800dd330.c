
/* WARNING: Removing unreachable block (ram,0x0001800dd897) */
/* WARNING: Removing unreachable block (ram,0x0001800dd89d) */
/* WARNING: Removing unreachable block (ram,0x0001800dd8ab) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined8 FUN_1800dd330(longlong param_1)

{
  longlong *plVar1;
  char cVar2;
  ulonglong uVar3;
  longlong *plVar4;
  code *pcVar5;
  double dVar6;
  double dVar7;
  bool bVar8;
  double dVar9;
  double dVar10;
  char cVar11;
  char cVar12;
  int iVar13;
  uint uVar14;
  int iVar15;
  undefined4 uVar16;
  undefined8 uVar17;
  undefined8 uVar18;
  undefined8 *puVar19;
  int *piVar20;
  longlong lVar21;
  ulonglong uVar22;
  float fVar23;
  undefined4 auStackX_10 [2];
  int aiStackX_18 [2];
  int aiStackX_20 [2];
  undefined8 in_stack_fffffffffffffec8;
  undefined4 uVar24;
  double in_stack_fffffffffffffed0;
  undefined4 uVar26;
  undefined8 uVar25;
  undefined8 uStack_118;
  longlong lStack_110;
  longlong lStack_108;
  longlong *plStack_100;
  longlong lStack_f8;
  longlong lStack_f0;
  longlong lStack_e8;
  longlong lStack_e0;
  longlong lStack_d8;
  longlong lStack_d0;
  longlong lStack_c8;
  longlong alStack_c0 [2];
  undefined1 auStack_b0 [8];
  longlong *plStack_a8;
  
  alStack_c0[1] = 0xfffffffffffffffe;
  *(undefined1 *)(param_1 + 0x1d0) = 0;
  iVar13 = ResetEvent(*(undefined8 *)(param_1 + 0x98));
  if (iVar13 == 0) {
    GetLastError();
  }
  cVar2 = *(char *)(param_1 + 0x7c);
  if (cVar2 != '\0') {
    if ((_DAT_1802b2980 & 1) == 0) {
      _DAT_1802b2980 = _DAT_1802b2980 | 1;
      FUN_18015a9d0();
      FUN_1801e8930(FUN_180202990);
    }
    FUN_18015b250(&DAT_1802b2990);
  }
  iVar13 = 0;
  dVar6 = 0.0;
  cVar12 = *(char *)(param_1 + 0x7c);
  cVar11 = FUN_18001dbc0(2);
  uVar24 = (undefined4)((ulonglong)in_stack_fffffffffffffec8 >> 0x20);
  uVar26 = (undefined4)((ulonglong)in_stack_fffffffffffffed0 >> 0x20);
  dVar9 = _DAT_18023a7b0;
  dVar10 = _DAT_18023a748;
  if (cVar11 != '\0') {
    uVar17 = FUN_18001db50();
    in_stack_fffffffffffffed0 = (double)CONCAT44(uVar26,*(undefined4 *)(param_1 + 0x50));
    in_stack_fffffffffffffec8 = CONCAT44(uVar24,0x15f);
    FUN_180015b40(uVar17,2,&UNK_18022cb18,&UNK_18022cb50,in_stack_fffffffffffffec8,
                  in_stack_fffffffffffffed0,cVar12);
    dVar9 = _DAT_18023a7b0;
    dVar10 = _DAT_18023a748;
  }
  do {
    if ((cVar12 == '\0') || (_DAT_1802a5690 == 0)) {
      uVar14 = 0;
      if ((_DAT_1802b3550 & 1) == 0) {
        _DAT_1802b3550 = _DAT_1802b3550 | 1;
        LOCK();
        _DAT_1802b3554 = 0;
        UNLOCK();
      }
      if (_DAT_1802a5694 < _DAT_1802b3554) {
        cVar11 = FUN_18001dbc0(4);
        uVar24 = (undefined4)((ulonglong)in_stack_fffffffffffffec8 >> 0x20);
        uVar26 = (undefined4)((ulonglong)in_stack_fffffffffffffed0 >> 0x20);
        if (cVar11 != '\0') {
          if ((_DAT_1802b3550 & 1) == 0) {
            _DAT_1802b3550 = _DAT_1802b3550 | 1;
            LOCK();
            _DAT_1802b3554 = 0;
            UNLOCK();
          }
          iVar15 = _DAT_1802b3554;
          if ((_DAT_1802b1218 & 1) == 0) {
            _DAT_1802b1218 = _DAT_1802b1218 | 1;
            _DAT_1802b1230 = 1;
            _DAT_1802b1238 = 0;
            _DAT_1802b1240 = 0;
            uRam00000001802b1248 = 0;
            _DAT_1802b1250 = 0;
            uRam00000001802b1258 = 0;
            _DAT_1802b1260 = 0;
            uRam00000001802b1268 = 0;
            _DAT_1802b1270 = 0;
            FUN_1801e8930(&UNK_180202850);
          }
          in_stack_fffffffffffffed0 = (double)CONCAT44(uVar26,iVar15);
          in_stack_fffffffffffffec8 = CONCAT44(uVar24,0x16f);
          FUN_180015b40(&DAT_1802b1230,4,&UNK_18022cb90,&UNK_18022cb50,in_stack_fffffffffffffec8,
                        in_stack_fffffffffffffed0,_DAT_1802a5694);
        }
        goto LAB_1800dd4d4;
      }
      uVar3 = *(ulonglong *)(param_1 + 0xb8);
      if (*(ulonglong *)(param_1 + 0xc0) < 0x10) {
        lVar21 = param_1 + 0xa8;
      }
      else {
        lVar21 = *(longlong *)(param_1 + 0xa8);
      }
      uVar22 = 0x18;
      if (uVar3 < 0x18) {
        uVar22 = uVar3;
      }
      if (uVar22 != 0) {
        uVar14 = func_0x0001801e92c0(lVar21,&UNK_18022cb68);
      }
      if (uVar14 == 0) {
        if (uVar3 < 0x18) {
          uVar14 = 0xffffffff;
        }
        else {
          uVar14 = (uint)(uVar3 != 0x18);
        }
      }
      if (uVar14 == 0) {
        *(undefined1 *)(param_1 + 0x14c) = 1;
      }
      else {
        *(undefined1 *)(param_1 + 0x14c) = 0;
        if ((_DAT_1802b1888 & 1) == 0) {
          _DAT_1802b1888 = _DAT_1802b1888 | 1;
          FUN_180107fc0();
          FUN_1801e8930(&UNK_180202980);
        }
        FUN_180108ba0(&DAT_1802b1890,&lStack_108,*(undefined4 *)(param_1 + 0x50));
        plVar4 = plStack_100;
        if (lStack_108 != 0) {
          LOCK();
          piVar20 = (int *)(lStack_108 + 0xe4);
          iVar13 = *piVar20;
          *piVar20 = *piVar20 + 1;
          UNLOCK();
          *(int *)(param_1 + 0xe0) = iVar13 + 1;
        }
        if (plStack_100 != (longlong *)0x0) {
          LOCK();
          plVar1 = plStack_100 + 1;
          *(int *)plVar1 = (int)*plVar1 + -1;
          UNLOCK();
          if ((int)*plVar1 == 0) {
            (**(code **)*plStack_100)(plStack_100);
            LOCK();
            piVar20 = (int *)((longlong)plVar4 + 0xc);
            *piVar20 = *piVar20 + -1;
            UNLOCK();
            if (*piVar20 == 0) {
              (**(code **)(*plStack_100 + 8))();
            }
          }
        }
      }
      FUN_1800e2430(param_1);
      FUN_1800e1f80(param_1);
      if (cVar12 != '\0') {
        if ((_DAT_1802b3358 & 1) == 0) {
          _DAT_1802b3358 = _DAT_1802b3358 | 1;
          FUN_18008f080();
          FUN_1801e8930(&UNK_180202f60);
        }
        uVar24 = *(undefined4 *)(param_1 + 0x50);
        auStackX_10[0] = uVar24;
        iVar13 = _Mtx_lock(0x1802b34e8);
        if (iVar13 != 0) {
          __Throw_C_error_std__YAXH_Z(iVar13);
        }
        FUN_18009ab20(&DAT_1802b33a8,&lStack_d0,auStackX_10);
        if (lStack_d0 != _DAT_1802b33a8) {
          FUN_1800f0690(&DAT_1802b33a8,auStackX_10);
        }
        FUN_18009ab20(&DAT_1802b3368,alStack_c0,auStackX_10);
        if (alStack_c0[0] != _DAT_1802b3368) {
          FUN_1800f04d0(&DAT_1802b3368,auStackX_10);
        }
        iVar13 = _Mtx_unlock(0x1802b34e8);
        if (iVar13 != 0) {
          __Throw_C_error_std__YAXH_Z(iVar13);
        }
        FUN_1800ef800(&DAT_1802b3360,uVar24);
        FUN_1800e0030(param_1);
      }
      FUN_1800e22f0(param_1);
      if (*(char *)(param_1 + 0x14c) == '\0') {
        FUN_1800e2d70(param_1 + 0x1e8);
      }
      FUN_1800de180(param_1,2,0,0);
      iVar13 = QueryPerformanceFrequency(&lStack_e8);
      dVar6 = 0.0;
      if ((iVar13 != 0) && (iVar13 = QueryPerformanceCounter(&lStack_d8), dVar6 = 0.0, iVar13 != 0))
      {
        dVar6 = ((double)lStack_d8 / (double)lStack_e8) * dVar9;
      }
      *(double *)(param_1 + 0x178) = dVar6;
      if ((_DAT_1802b2f70 & 1) == 0) {
        _DAT_1802b2f70 = _DAT_1802b2f70 | 1;
        FUN_180093820();
        FUN_1801e8930(FUN_180202e40);
      }
      FUN_180093900(&DAT_1802b2f80);
      *(undefined4 *)(param_1 + 0xd8) = 2;
      lStack_110 = *(longlong *)(*(longlong *)(param_1 + 200) + 0x10);
      uStack_118 = *(undefined8 *)(*(longlong *)(param_1 + 200) + 8);
      if (lStack_110 != 0) {
        LOCK();
        *(int *)(lStack_110 + 8) = *(int *)(lStack_110 + 8) + 1;
        UNLOCK();
      }
      FUN_1800ffaa0(param_1 + 200,&uStack_118,0);
      (**(code **)(**(longlong **)(param_1 + 200) + 8))();
      *(undefined4 *)(param_1 + 0xd8) = 1;
      iVar13 = QueryPerformanceFrequency(&lStack_f8);
      in_stack_fffffffffffffed0 = 0.0;
      if ((iVar13 != 0) &&
         (iVar13 = QueryPerformanceCounter(&lStack_e0), in_stack_fffffffffffffed0 = 0.0, iVar13 != 0
         )) {
        in_stack_fffffffffffffed0 = ((double)lStack_e0 / (double)lStack_f8) * dVar9;
      }
      if (*(char *)(param_1 + 0x14c) == '\0') {
        dVar6 = in_stack_fffffffffffffed0 - *(double *)(param_1 + 0x178);
      }
      else {
        dVar6 = 0.0;
      }
      fVar23 = (float)dVar6;
      uVar18 = FUN_18008e6c0(*(undefined8 *)(param_1 + 0x198));
      uVar17 = *(undefined8 *)(param_1 + 0x178);
      FUN_1800c9ea0(uVar18,*(undefined4 *)(param_1 + 0x50),fVar23,fVar23,uVar17,
                    in_stack_fffffffffffffed0);
      FUN_1800e00e0(param_1);
      uVar18 = FUN_18008e6c0(0);
      FUN_1800ce740(uVar18,*(undefined4 *)(param_1 + 0x50),*(undefined4 *)(param_1 + 0xe0));
      if (*(char *)(param_1 + 0x14c) == '\0') {
        plVar4 = *(longlong **)(param_1 + 0x1e8);
        if (plVar4 != (longlong *)0x0) {
          (**(code **)(*plVar4 + 0x18))(plVar4,&UNK_18022d720,&UNK_18022d708,8);
        }
        FUN_1800e2fd0(param_1 + 0x1e8);
        FUN_1800e2160(param_1);
      }
      puVar19 = (undefined8 *)FUN_1800d33b0(auStack_b0);
      FUN_1800d53c0(*puVar19,*(undefined4 *)(param_1 + 0x50),*(undefined8 *)(param_1 + 0x198));
      plVar4 = plStack_a8;
      if (plStack_a8 != (longlong *)0x0) {
        LOCK();
        plVar1 = plStack_a8 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plStack_a8)(plStack_a8);
          LOCK();
          piVar20 = (int *)((longlong)plVar4 + 0xc);
          *piVar20 = *piVar20 + -1;
          UNLOCK();
          if (*piVar20 == 0) {
            (**(code **)(*plVar4 + 8))(plVar4);
          }
        }
      }
      if (cVar12 != '\0') {
        FUN_1800de180(param_1,1,fVar23);
      }
      if ((_DAT_1802b3080 & 1) == 0) {
        _DAT_1802b3080 = _DAT_1802b3080 | 1;
        FUN_180082fe0();
        FUN_1801e8930(&UNK_180202f00);
      }
      FUN_180083150(&DAT_1802b3088,*(undefined4 *)(param_1 + 0x50));
      if (*(char *)(param_1 + 0x14c) == '\0') {
        FUN_1800defb0(param_1);
      }
      FUN_1800dfdd0(param_1);
      if (*(char *)(param_1 + 0x114) != '\0') {
        cVar11 = FUN_1800de780(param_1);
        uVar24 = (undefined4)((ulonglong)uVar17 >> 0x20);
        uVar26 = (undefined4)((ulonglong)in_stack_fffffffffffffed0 >> 0x20);
        if (cVar11 != '\0') {
          bVar8 = true;
          cVar11 = FUN_18001dbc0(2);
          if (cVar11 != '\0') {
            uVar17 = FUN_18001db50();
            uVar25 = CONCAT44(uVar26,*(undefined4 *)(param_1 + 0x50));
            uVar18 = CONCAT44(uVar24,0x1cd);
            FUN_180015b40(uVar17,2,&UNK_18022cc18,&UNK_18022cb50,uVar18,uVar25);
            uVar24 = (undefined4)((ulonglong)uVar18 >> 0x20);
            uVar26 = (undefined4)((ulonglong)uVar25 >> 0x20);
          }
          goto LAB_1800ddbe1;
        }
      }
      if ((_DAT_1802b3120 & 1) == 0) {
        _DAT_1802b3120 = _DAT_1802b3120 | 1;
        FUN_1800e3630();
        FUN_1801e8930(&UNK_180202f30);
      }
      in_stack_fffffffffffffec8 =
           CONCAT71((int7)((ulonglong)uVar17 >> 8),*(undefined1 *)(param_1 + 0x1e0));
      FUN_1800e4080(&DAT_1802b3128,*(undefined4 *)(param_1 + 0x50),*(undefined8 *)(param_1 + 0x198),
                    param_1 + 0xa8,in_stack_fffffffffffffec8);
      FUN_1800e32c0(param_1 + 0x1e8);
      iVar13 = QueryPerformanceFrequency(&lStack_f0);
      dVar7 = 0.0;
      if ((iVar13 != 0) && (iVar13 = QueryPerformanceCounter(&lStack_c8), dVar7 = 0.0, iVar13 != 0))
      {
        dVar7 = ((double)lStack_c8 / (double)lStack_f0) * dVar9;
      }
      *(double *)(param_1 + 0x1d8) = dVar7;
      iVar13 = 0;
      if (0 < *(int *)(param_1 + 0x78)) {
        aiStackX_18[0] = 0;
        aiStackX_20[0] =
             *(int *)(param_1 + 0x78) - (int)((dVar7 - *(double *)(param_1 + 0x178)) + dVar10);
        piVar20 = aiStackX_18;
        if (-1 < aiStackX_20[0]) {
          piVar20 = aiStackX_20;
        }
        iVar13 = *piVar20;
      }
    }
    else {
      cVar11 = FUN_18001dbc0(4);
      uVar24 = (undefined4)((ulonglong)in_stack_fffffffffffffec8 >> 0x20);
      if (cVar11 != '\0') {
        if ((_DAT_1802b1218 & 1) == 0) {
          _DAT_1802b1218 = _DAT_1802b1218 | 1;
          _DAT_1802b1230 = 1;
          _DAT_1802b1238 = 0;
          _DAT_1802b1240 = 0;
          uRam00000001802b1248 = 0;
          _DAT_1802b1250 = 0;
          uRam00000001802b1258 = 0;
          _DAT_1802b1260 = 0;
          uRam00000001802b1268 = 0;
          _DAT_1802b1270 = 0;
          FUN_1801e8930(&UNK_180202850);
        }
        in_stack_fffffffffffffec8 = CONCAT44(uVar24,0x166);
        FUN_180015b40(&DAT_1802b1230,4,&UNK_18022c920,&UNK_18022cb50,in_stack_fffffffffffffec8);
      }
LAB_1800dd4d4:
      FUN_1800da1c0(*(undefined8 *)(param_1 + 0x90),(longlong)*(int *)(param_1 + 0x78));
    }
    uVar24 = (undefined4)((ulonglong)in_stack_fffffffffffffec8 >> 0x20);
    uVar26 = (undefined4)((ulonglong)in_stack_fffffffffffffed0 >> 0x20);
    if (cVar12 == '\0') break;
    iVar15 = FUN_1800da1c0(*(undefined8 *)(param_1 + 0x80),(longlong)iVar13);
    uVar24 = (undefined4)((ulonglong)in_stack_fffffffffffffec8 >> 0x20);
    uVar26 = (undefined4)((ulonglong)in_stack_fffffffffffffed0 >> 0x20);
  } while (iVar15 != 0);
  bVar8 = false;
LAB_1800ddbe1:
  iVar13 = SetEvent(*(undefined8 *)(param_1 + 0x88));
  if (iVar13 == 0) {
    GetLastError();
  }
  *(undefined1 *)(param_1 + 0xe9) = 0;
  if (cVar12 == '\0') {
    FUN_1800de180(param_1,1,(float)dVar6);
  }
  *(undefined1 *)(param_1 + 0x7c) = 0;
  *(undefined4 *)(param_1 + 300) = 1;
  if (*(char *)(param_1 + 0x128) == '\0') {
    uVar16 = *(undefined4 *)(param_1 + 300);
    if (bVar8) {
      uVar16 = 2;
    }
    *(undefined4 *)(param_1 + 300) = uVar16;
  }
  else {
    *(undefined4 *)(param_1 + 300) = 3;
  }
  FUN_1800de180(param_1,3,0,*(undefined4 *)(param_1 + 300));
  if (*(longlong *)(param_1 + 0x1c8) != 0) {
    if (*(longlong **)(param_1 + 0x1c8) == (longlong *)0x0) {
      __Xbad_function_call_std__YAXXZ();
      pcVar5 = (code *)swi(3);
      uVar17 = (*pcVar5)();
      return uVar17;
    }
    (**(code **)(**(longlong **)(param_1 + 0x1c8) + 0x10))();
  }
  *(undefined1 *)(param_1 + 0x1d0) = 1;
  iVar13 = SetEvent(*(undefined8 *)(param_1 + 0x98));
  if (iVar13 == 0) {
    GetLastError();
  }
  cVar12 = FUN_18001dbc0(2);
  if (cVar12 != '\0') {
    uVar17 = FUN_18001db50();
    FUN_180015b40(uVar17,2,&UNK_18022cbf0,&UNK_18022cb50,CONCAT44(uVar24,0x200),
                  CONCAT44(uVar26,*(undefined4 *)(param_1 + 0x50)));
  }
  if (cVar2 != '\0') {
    if ((_DAT_1802b2980 & 1) == 0) {
      _DAT_1802b2980 = _DAT_1802b2980 | 1;
      FUN_18015a9d0();
      FUN_1801e8930(FUN_180202990);
    }
    iVar13 = _Mtx_lock(0x1802b2ac0);
    if (iVar13 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar13);
    }
    _DAT_1802b2ab4 = _DAT_1802b2ab4 + -1;
    iVar13 = _Mtx_unlock(0x1802b2ac0);
    if (iVar13 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar13);
    }
  }
  return 0;
}

