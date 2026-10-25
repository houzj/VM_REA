
undefined8 * FUN_18006f290(longlong *param_1,undefined8 *param_2,longlong *param_3)

{
  int *piVar1;
  char cVar2;
  longlong *plVar3;
  longlong lVar4;
  undefined8 *puVar5;
  longlong *plVar6;
  longlong *plVar7;
  longlong *plVar8;
  longlong *plVar9;
  longlong *plStackX_18;
  
  plStackX_18 = param_3;
  if (*(char *)((longlong)param_3 + 0x19) == '\0') {
    plStackX_18 = (longlong *)param_3[2];
    if (*(char *)((longlong)plStackX_18 + 0x19) == '\0') {
      cVar2 = *(char *)(*plStackX_18 + 0x19);
      plVar8 = (longlong *)*plStackX_18;
      while (cVar2 == '\0') {
        cVar2 = *(char *)(*plVar8 + 0x19);
        plStackX_18 = plVar8;
        plVar8 = (longlong *)*plVar8;
      }
    }
    else {
      cVar2 = *(char *)(param_3[1] + 0x19);
      plVar9 = (longlong *)param_3[1];
      plVar8 = param_3;
      while ((plStackX_18 = plVar9, cVar2 == '\0' && (plVar8 == (longlong *)plStackX_18[2]))) {
        cVar2 = *(char *)(plStackX_18[1] + 0x19);
        plVar9 = (longlong *)plStackX_18[1];
        plVar8 = plStackX_18;
      }
    }
  }
  plVar8 = (longlong *)*param_3;
  if (*(char *)((longlong)plVar8 + 0x19) == '\0') {
    plVar9 = plVar8;
    if ((*(char *)(param_3[2] + 0x19) == '\0') &&
       (plVar9 = (longlong *)plStackX_18[2], plStackX_18 != param_3)) {
      plVar8[1] = (longlong)plStackX_18;
      *plStackX_18 = *param_3;
      plVar8 = plStackX_18;
      if (plStackX_18 != (longlong *)param_3[2]) {
        plVar8 = (longlong *)plStackX_18[1];
        if (*(char *)((longlong)plVar9 + 0x19) == '\0') {
          plVar9[1] = (longlong)plVar8;
        }
        *plVar8 = (longlong)plVar9;
        plStackX_18[2] = param_3[2];
        *(longlong **)(param_3[2] + 8) = plStackX_18;
      }
      if (*(longlong **)(*param_1 + 8) == param_3) {
        *(longlong **)(*param_1 + 8) = plStackX_18;
      }
      else {
        puVar5 = (undefined8 *)param_3[1];
        if ((longlong *)*puVar5 == param_3) {
          *puVar5 = plStackX_18;
        }
        else {
          puVar5[2] = plStackX_18;
        }
      }
      plStackX_18[1] = param_3[1];
      lVar4 = plStackX_18[3];
      *(char *)(plStackX_18 + 3) = (char)param_3[3];
      *(char *)(param_3 + 3) = (char)lVar4;
      goto LAB_18006f452;
    }
  }
  else {
    plVar9 = (longlong *)param_3[2];
  }
  plVar8 = (longlong *)param_3[1];
  if (*(char *)((longlong)plVar9 + 0x19) == '\0') {
    plVar9[1] = (longlong)plVar8;
  }
  if (*(longlong **)(*param_1 + 8) == param_3) {
    *(longlong **)(*param_1 + 8) = plVar9;
  }
  else if ((longlong *)*plVar8 == param_3) {
    *plVar8 = (longlong)plVar9;
  }
  else {
    plVar8[2] = (longlong)plVar9;
  }
  if (*(longlong **)*param_1 == param_3) {
    plVar7 = plVar8;
    if (*(char *)((longlong)plVar9 + 0x19) == '\0') {
      cVar2 = *(char *)(*plVar9 + 0x19);
      plVar3 = (longlong *)*plVar9;
      plVar7 = plVar9;
      while (plVar6 = plVar3, cVar2 == '\0') {
        plVar3 = (longlong *)*plVar6;
        cVar2 = *(char *)((longlong)plVar3 + 0x19);
        plVar7 = plVar6;
      }
    }
    *(longlong **)*param_1 = plVar7;
  }
  lVar4 = *param_1;
  if (*(longlong **)(lVar4 + 0x10) == param_3) {
    if (*(char *)((longlong)plVar9 + 0x19) == '\0') {
      cVar2 = *(char *)(plVar9[2] + 0x19);
      plVar7 = (longlong *)plVar9[2];
      plVar3 = plVar9;
      while (plVar6 = plVar7, cVar2 == '\0') {
        plVar7 = (longlong *)plVar6[2];
        cVar2 = *(char *)((longlong)plVar7 + 0x19);
        plVar3 = plVar6;
      }
      *(longlong **)(lVar4 + 0x10) = plVar3;
    }
    else {
      *(longlong **)(lVar4 + 0x10) = plVar8;
    }
  }
LAB_18006f452:
  if ((char)param_3[3] == '\x01') {
    if (plVar9 != *(longlong **)(*param_1 + 8)) {
      do {
        plVar7 = plVar8;
        if ((char)plVar9[3] != '\x01') break;
        plVar8 = (longlong *)*plVar7;
        if (plVar9 == plVar8) {
          plVar8 = (longlong *)plVar7[2];
          if ((char)plVar8[3] == '\0') {
            *(undefined1 *)(plVar8 + 3) = 1;
            plVar8 = (longlong *)plVar7[2];
            *(undefined1 *)(plVar7 + 3) = 0;
            plVar7[2] = *plVar8;
            if (*(char *)(*plVar8 + 0x19) == '\0') {
              *(longlong **)(*plVar8 + 8) = plVar7;
            }
            plVar8[1] = plVar7[1];
            if (plVar7 == *(longlong **)(*param_1 + 8)) {
              *(longlong **)(*param_1 + 8) = plVar8;
            }
            else {
              puVar5 = (undefined8 *)plVar7[1];
              if (plVar7 == (longlong *)*puVar5) {
                *puVar5 = plVar8;
              }
              else {
                puVar5[2] = plVar8;
              }
            }
            *plVar8 = (longlong)plVar7;
            plVar7[1] = (longlong)plVar8;
            plVar8 = (longlong *)plVar7[2];
          }
          if (*(char *)((longlong)plVar8 + 0x19) == '\0') {
            if ((*(char *)(*plVar8 + 0x18) != '\x01') || (*(char *)(plVar8[2] + 0x18) != '\x01')) {
              if (*(char *)(plVar8[2] + 0x18) == '\x01') {
                *(undefined1 *)(*plVar8 + 0x18) = 1;
                *(undefined1 *)(plVar8 + 3) = 0;
                FUN_180020170(param_1,plVar8);
                plVar8 = (longlong *)plVar7[2];
              }
              *(char *)(plVar8 + 3) = (char)plVar7[3];
              *(undefined1 *)(plVar7 + 3) = 1;
              *(undefined1 *)(plVar8[2] + 0x18) = 1;
              FUN_180020110(param_1,plVar7);
              break;
            }
LAB_18006f57d:
            *(undefined1 *)(plVar8 + 3) = 0;
          }
        }
        else {
          if ((char)plVar8[3] == '\0') {
            *(undefined1 *)(plVar8 + 3) = 1;
            *(undefined1 *)(plVar7 + 3) = 0;
            FUN_180020170(param_1,plVar7);
            plVar8 = (longlong *)*plVar7;
          }
          if (*(char *)((longlong)plVar8 + 0x19) == '\0') {
            if ((*(char *)(plVar8[2] + 0x18) == '\x01') && (*(char *)(*plVar8 + 0x18) == '\x01'))
            goto LAB_18006f57d;
            if (*(char *)(*plVar8 + 0x18) == '\x01') {
              *(undefined1 *)(plVar8[2] + 0x18) = 1;
              *(undefined1 *)(plVar8 + 3) = 0;
              FUN_180020110(param_1,plVar8);
              plVar8 = (longlong *)*plVar7;
            }
            *(char *)(plVar8 + 3) = (char)plVar7[3];
            *(undefined1 *)(plVar7 + 3) = 1;
            *(undefined1 *)(*plVar8 + 0x18) = 1;
            FUN_180020170(param_1,plVar7);
            break;
          }
        }
        plVar8 = (longlong *)plVar7[1];
        plVar9 = plVar7;
      } while (plVar7 != *(longlong **)(*param_1 + 8));
    }
    *(undefined1 *)(plVar9 + 3) = 1;
  }
  plVar8 = (longlong *)param_3[9];
  if (plVar8 != (longlong *)0x0) {
    LOCK();
    plVar9 = plVar8 + 1;
    *(int *)plVar9 = (int)*plVar9 + -1;
    UNLOCK();
    if ((int)*plVar9 == 0) {
      (**(code **)*plVar8)(plVar8);
      LOCK();
      piVar1 = (int *)((longlong)plVar8 + 0xc);
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (*piVar1 == 0) {
        (**(code **)(*plVar8 + 8))(plVar8);
      }
    }
  }
  if (0xf < (ulonglong)param_3[7]) {
    __3_YAXPEAX_Z(param_3[4]);
  }
  param_3[7] = 0xf;
  param_3[6] = 0;
  *(undefined1 *)(param_3 + 4) = 0;
  __3_YAXPEAX_Z(param_3);
  if (param_1[1] != 0) {
    param_1[1] = param_1[1] + -1;
  }
  *param_2 = plStackX_18;
  return param_2;
}

