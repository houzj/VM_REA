
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180080080(longlong param_1)

{
  int *piVar1;
  longlong *plVar2;
  double dVar3;
  double dVar4;
  double dVar5;
  longlong lVar6;
  char cVar7;
  uint uVar8;
  int iVar9;
  undefined8 uVar10;
  longlong lVar11;
  longlong *plVar12;
  longlong *plVar13;
  longlong lVar14;
  longlong *plVar15;
  longlong lVar16;
  undefined1 auStack_198 [32];
  undefined4 uStack_178;
  longlong lStack_170;
  double dStack_168;
  longlong alStack_158 [3];
  longlong *plStack_140;
  longlong lStack_138;
  longlong *plStack_130;
  longlong *plStack_128;
  longlong lStack_120;
  undefined1 auStack_118 [24];
  longlong *plStack_100;
  longlong *plStack_f8;
  longlong lStack_f0;
  longlong *plStack_e8;
  undefined8 uStack_e0;
  longlong lStack_d8;
  longlong lStack_d0;
  longlong alStack_c8 [3];
  longlong *plStack_b0;
  undefined1 auStack_a8 [48];
  longlong *plStack_78;
  ulonglong uStack_68;
  
  uStack_e0 = 0xfffffffffffffffe;
  uStack_68 = _DAT_1802a5230 ^ (ulonglong)auStack_198;
  cVar7 = FUN_18001dbc0(2);
  if (cVar7 != '\0') {
    uVar10 = FUN_18001db50();
    uStack_178 = 0x72;
    FUN_180015b40(uVar10,2,&UNK_180224e10,&UNK_180224df0);
  }
  dVar5 = _DAT_18023a7b0;
  dVar4 = _DAT_18023a748;
  if (*(char *)(param_1 + 0x110) != '\0') {
    lVar16 = 0;
    do {
      if (*(longlong *)(param_1 + 0x28) == *(longlong *)(param_1 + 0x30)) {
        uVar8 = WaitForSingleObject(*(undefined8 *)(param_1 + 0x100));
        if ((uVar8 & 0xffffff7f) != 0) {
          if (uVar8 == 0x102) {
            iVar9 = 0x102;
          }
          else {
            iVar9 = GetLastError();
            if (iVar9 == 0) goto LAB_18008072f;
          }
          cVar7 = FUN_18001dbc0(4);
          if (cVar7 != '\0') {
            uVar10 = FUN_18001db50();
            lStack_170 = CONCAT44(lStack_170._4_4_,iVar9);
            uStack_178 = 0x7a;
            FUN_180015b40(uVar10,4,&UNK_180224e30,&UNK_180224df0);
          }
        }
      }
      else {
        plStack_b0 = (longlong *)0x0;
        lStack_f0 = param_1;
        iVar9 = _Mtx_lock(param_1);
        if (iVar9 != 0) {
          __Throw_C_error_std__YAXH_Z(iVar9);
        }
        if (*(longlong *)(param_1 + 0x28) != *(longlong *)(param_1 + 0x30)) {
          do {
            plVar12 = *(longlong **)(param_1 + 0x28);
            lStack_d8 = *plVar12;
            lStack_d0 = plVar12[1];
            if (alStack_c8 != plVar12 + 2) {
              if (plStack_b0 != (longlong *)0x0) {
                (**(code **)(*plStack_b0 + 0x20))
                          (plStack_b0,
                           CONCAT71((int7)((ulonglong)alStack_c8 >> 8),plStack_b0 != alStack_c8));
                plStack_b0 = (longlong *)0x0;
              }
              plVar13 = (longlong *)plVar12[5];
              if (plVar13 == (longlong *)0x0) {
                plStack_b0 = (longlong *)0x0;
              }
              else {
                if (plVar13 == plVar12 + 2) {
                  plVar12 = alStack_c8;
                }
                else {
                  plVar12 = (longlong *)0x0;
                }
                plStack_b0 = (longlong *)(**(code **)*plVar13)(plVar13,plVar12);
              }
            }
            cVar7 = FUN_180080a30(param_1,lStack_d0,0);
            lVar14 = lStack_d8;
            if (cVar7 == '\0') {
              iVar9 = QueryPerformanceCounter(&lStack_138);
              lVar11 = lStack_138;
              if (iVar9 == 0) {
                if ((_DAT_1802b1218 & 1) == 0) {
                  _DAT_1802b1218 = _DAT_1802b1218 | 1;
                  _DAT_1802b1230 = 1;
                  _DAT_1802b1238 = 0;
                  _DAT_1802b1240 = (longlong *)0x0;
                  uRam00000001802b1248 = 0;
                  _DAT_1802b1250 = 0;
                  uRam00000001802b1258 = 0;
                  _DAT_1802b1260 = 0;
                  uRam00000001802b1268 = 0;
                  _DAT_1802b1270 = 0;
                  FUN_1801e8930(&UNK_180202850);
                }
                plVar12 = _DAT_1802b1240;
                lVar6 = _DAT_1802b1238;
                if (_DAT_1802b1240 != (longlong *)0x0) {
                  LOCK();
                  *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
                  UNLOCK();
                }
                lVar11 = lVar16;
                if (lVar6 == 0) {
                  if (plVar12 != (longlong *)0x0) {
                    LOCK();
                    plVar13 = plVar12 + 1;
                    *(int *)plVar13 = (int)*plVar13 + -1;
                    UNLOCK();
                    if ((int)*plVar13 == 0) {
                      (**(code **)*plVar12)(plVar12);
                      LOCK();
                      piVar1 = (int *)((longlong)plVar12 + 0xc);
                      *piVar1 = *piVar1 + -1;
                      UNLOCK();
                      if (*piVar1 == 0) {
                        (**(code **)(*plVar12 + 8))(plVar12);
                      }
                    }
                  }
                }
                else {
                  iVar9 = *(int *)(lVar6 + 0x40);
                  if (plVar12 != (longlong *)0x0) {
                    LOCK();
                    plVar13 = plVar12 + 1;
                    *(int *)plVar13 = (int)*plVar13 + -1;
                    UNLOCK();
                    if ((int)*plVar13 == 0) {
                      (**(code **)*plVar12)(plVar12);
                      LOCK();
                      piVar1 = (int *)((longlong)plVar12 + 0xc);
                      *piVar1 = *piVar1 + -1;
                      UNLOCK();
                      if (*piVar1 == 0) {
                        (**(code **)(*plVar12 + 8))(plVar12);
                      }
                    }
                  }
                  if (iVar9 < 5) {
                    if ((_DAT_1802b1218 & 1) == 0) {
                      _DAT_1802b1218 = _DAT_1802b1218 | 1;
                      _DAT_1802b1230 = 1;
                      _DAT_1802b1238 = 0;
                      _DAT_1802b1240 = (longlong *)0x0;
                      uRam00000001802b1248 = 0;
                      _DAT_1802b1250 = 0;
                      uRam00000001802b1258 = 0;
                      _DAT_1802b1260 = 0;
                      uRam00000001802b1268 = 0;
                      _DAT_1802b1270 = 0;
                      FUN_1801e8930(&UNK_180202850);
                    }
                    uStack_178 = 0x14a;
                    FUN_180015b40(&DAT_1802b1230,4,&UNK_180224f90,&UNK_180224fb0);
                  }
                }
              }
              dVar3 = ((double)(lVar14 - lVar11) / (double)*(longlong *)(param_1 + 0xf0)) * dVar5;
              if (dVar4 <= dVar3) break;
              if ((_DAT_1802b1218 & 1) == 0) {
                _DAT_1802b1218 = _DAT_1802b1218 | 1;
                _DAT_1802b1230 = 1;
                _DAT_1802b1238 = 0;
                _DAT_1802b1240 = (longlong *)0x0;
                uRam00000001802b1248 = 0;
                _DAT_1802b1250 = 0;
                uRam00000001802b1258 = 0;
                _DAT_1802b1260 = 0;
                uRam00000001802b1268 = 0;
                _DAT_1802b1270 = 0;
                FUN_1801e8930(&UNK_180202850);
              }
              plVar12 = _DAT_1802b1240;
              lVar14 = _DAT_1802b1238;
              if (_DAT_1802b1240 != (longlong *)0x0) {
                LOCK();
                *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
                UNLOCK();
              }
              if (lVar14 == 0) {
                if (plVar12 != (longlong *)0x0) {
                  LOCK();
                  plVar13 = plVar12 + 1;
                  *(int *)plVar13 = (int)*plVar13 + -1;
                  UNLOCK();
                  if ((int)*plVar13 == 0) {
                    (**(code **)*plVar12)(plVar12);
                    LOCK();
                    piVar1 = (int *)((longlong)plVar12 + 0xc);
                    *piVar1 = *piVar1 + -1;
                    UNLOCK();
                    if (*piVar1 == 0) {
                      (**(code **)(*plVar12 + 8))(plVar12);
                    }
                  }
                }
              }
              else {
                iVar9 = *(int *)(lVar14 + 0x40);
                if (plVar12 != (longlong *)0x0) {
                  LOCK();
                  plVar13 = plVar12 + 1;
                  *(int *)plVar13 = (int)*plVar13 + -1;
                  UNLOCK();
                  if ((int)*plVar13 == 0) {
                    (**(code **)*plVar12)(plVar12);
                    LOCK();
                    piVar1 = (int *)((longlong)plVar12 + 0xc);
                    *piVar1 = *piVar1 + -1;
                    UNLOCK();
                    if (*piVar1 == 0) {
                      (**(code **)(*plVar12 + 8))(plVar12);
                    }
                  }
                }
                if (iVar9 < 3) {
                  if ((_DAT_1802b1218 & 1) == 0) {
                    _DAT_1802b1218 = _DAT_1802b1218 | 1;
                    _DAT_1802b1230 = 1;
                    _DAT_1802b1238 = 0;
                    _DAT_1802b1240 = (longlong *)0x0;
                    uRam00000001802b1248 = 0;
                    _DAT_1802b1250 = 0;
                    uRam00000001802b1258 = 0;
                    _DAT_1802b1260 = 0;
                    uRam00000001802b1268 = 0;
                    _DAT_1802b1270 = 0;
                    FUN_1801e8930(&UNK_180202850);
                  }
                  lStack_170 = lStack_d0;
                  uStack_178 = 0x97;
                  dStack_168 = dVar3;
                  FUN_180015b40(&DAT_1802b1230,2,&UNK_180224e58,&UNK_180224df0);
                }
              }
              plStack_e8 = alStack_158;
              lStack_120 = param_1;
              plVar12 = (longlong *)FUN_1800820a0(auStack_a8);
              plStack_140 = (longlong *)0x0;
              plVar13 = (longlong *)__2_YAPEAX_K_Z(0x50);
              if (plVar13 == (longlong *)0x0) {
                __Xbad_alloc_std__YAXXZ();
                goto LAB_180080745;
              }
              *plVar13 = (longlong)&UNK_180225030;
              plVar13[1] = *plVar12;
              plVar13[2] = plVar12[1];
              plVar13[3] = plVar12[2];
              plStack_128 = plVar13 + 4;
              plVar13[7] = 0;
              plVar2 = (longlong *)plVar12[6];
              plStack_130 = plVar13;
              plStack_f8 = plVar13;
              if (plVar2 == (longlong *)0x0) {
                plVar13[7] = 0;
              }
              else {
                plVar15 = plStack_128;
                if (plVar2 != plVar12 + 3) {
                  plVar15 = (longlong *)0x0;
                }
                lVar14 = (**(code **)*plVar2)(plVar2,plVar15);
                plVar13[7] = lVar14;
              }
              plVar13[8] = plVar12[7];
              plStack_128 = alStack_158;
              plStack_140 = plVar13;
              if ((_DAT_1802b2980 & 1) == 0) {
                _DAT_1802b2980 = _DAT_1802b2980 | 1;
                FUN_18015a9d0();
                FUN_1801e8930(FUN_180202990);
              }
              plStack_130 = (longlong *)auStack_118;
              plStack_100 = (longlong *)0x0;
              if (plStack_140 == (longlong *)0x0) {
                plStack_100 = (longlong *)0x0;
              }
              else {
                plVar12 = plStack_140;
                if (plStack_140 == alStack_158) {
                  plStack_100 = (longlong *)(**(code **)(*plStack_140 + 8))(plStack_140,auStack_118)
                  ;
                  if (plStack_140 == (longlong *)0x0) goto LAB_180080691;
                  (**(code **)(*plStack_140 + 0x20))
                            (plStack_140,
                             CONCAT71((int7)((ulonglong)alStack_158 >> 8),plStack_140 != alStack_158
                                     ));
                  plVar12 = plStack_100;
                }
                plStack_100 = plVar12;
                plStack_140 = (longlong *)0x0;
              }
LAB_180080691:
              FUN_1800fb9d0(0x1802b2998);
              if (plStack_140 != (longlong *)0x0) {
                (**(code **)(*plStack_140 + 0x20))();
                plStack_140 = (longlong *)0x0;
              }
              if (plStack_78 != (longlong *)0x0) {
                (**(code **)(*plStack_78 + 0x20))();
              }
            }
            FUN_180081480(param_1 + 0x28);
          } while (*(longlong *)(param_1 + 0x28) != *(longlong *)(param_1 + 0x30));
        }
        iVar9 = _Mtx_unlock(param_1);
        if (iVar9 != 0) {
          __Throw_C_error_std__YAXH_Z(iVar9);
        }
        FUN_1800808c0(param_1);
        if (plStack_b0 != (longlong *)0x0) {
          (**(code **)(*plStack_b0 + 0x20))();
        }
      }
LAB_18008072f:
    } while (*(char *)(param_1 + 0x110) != '\0');
  }
LAB_180080745:
  iVar9 = SetEvent(*(undefined8 *)(param_1 + 0x108));
  if (iVar9 == 0) {
    GetLastError();
  }
  cVar7 = FUN_18001dbc0(2);
  if (cVar7 != '\0') {
    uVar10 = FUN_18001db50();
    uStack_178 = 0xa7;
    FUN_180015b40(uVar10,2,&UNK_180224e88,&UNK_180224df0);
  }
  FUN_1801e8810(uStack_68 ^ (ulonglong)auStack_198);
  return;
}

