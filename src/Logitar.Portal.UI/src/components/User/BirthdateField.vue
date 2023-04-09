<template>
  <form-datetime id="birthdate" label="user.birthdate" :maxDate="new Date()" :value="value" @input="$emit('input', $event)" />
</template>

<script>
export default {
  name: 'BirthdateField',
  props: {
    value: {}
  }
}
</script>
