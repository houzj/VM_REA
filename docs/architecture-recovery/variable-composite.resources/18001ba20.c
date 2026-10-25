
longlong * FUN_18001ba20(longlong *param_1,uint param_2)

{
  if ((param_2 & 2) == 0) {
    FUN_18001b830(param_1);
    if ((param_2 & 1) != 0) {
      operator_delete(param_1);
    }
  }
  else {
    _eh_vector_destructor_iterator_(param_1,0x60,(int)param_1[-1],FUN_18001b830);
    if ((param_2 & 1) != 0) {
      operator_delete__(param_1 + -1);
    }
    param_1 = param_1 + -1;
  }
  return param_1;
}

