<template>
  <form-select id="sort" label="sort.label" :options="options" placeholder="sort.placeholder" :value="value" @input="$emit('input', $event)">
    <template #after>
      <b-form-checkbox :checked="desc" @input="$emit('desc', $event)">{{ $t('sort.desc') }}</b-form-checkbox>
    </template>
  </form-select>
</template>

<script>
export default {
  name: 'SortSelect',
  props: {
    desc: {
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
