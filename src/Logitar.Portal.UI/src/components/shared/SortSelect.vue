<template>
  <form-select id="sort" label="sort.label" :options="options" placeholder="sort.placeholder" :value="value" @input="$emit('input', $event)">
    <template #after>
      <b-form-checkbox :checked="isDescending" @input="$emit('isDescending', $event)">{{ $t('sort.isDescending') }}</b-form-checkbox>
    </template>
  </form-select>
</template>

<script>
export default {
  name: 'SortSelect',
  props: {
    isDescending: {
      type: Boolean,
      default: false
    },
    options: {
      type: Array,
      default: () => []
    },
    value: {}
  }
}
</script>
