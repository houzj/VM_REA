
void * FUN_18001bc70(void *param_1,uint param_2)

{
  if ((param_2 & 2) == 0) {
    FUN_18001b900((longlong)param_1);
    if ((param_2 & 1) != 0) {
      operator_delete(param_1);
    }
  }
  else {
    _eh_vector_destructor_iterator_(param_1,0x130,*(int *)((longlong)param_1 + -8),FUN_18001b900);
    if ((param_2 & 1) != 0) {
      operator_delete__((void *)((longlong)param_1 + -8));
    }
    param_1 = (void *)((longlong)param_1 + -8);
  }
  return param_1;
}

