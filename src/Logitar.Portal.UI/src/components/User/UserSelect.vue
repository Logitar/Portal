<template>
  <form-select id="user" label="user.select.label" :options="options" placeholder="user.select.placeholder" :value="value" @input="$emit('input', $event)" />
</template>

<script>
import { getUsers } from '@/api/users'

export default {
  name: 'UserSelect',
  props: {
    realm: {
      type: String,
      default: ''
    },
    value: {}
  },
  data() {
    return {
      users: []
    }
  },
  computed: {
    options() {
      return this.users.map(({ id, fullName, username }) => ({
        text: fullName ? `${fullName} (${username})` : username,
        value: id
      }))
    }
  },
  methods: {
    async refresh() {
      try {
        const { data } = await getUsers({
          realm: this.realm,
          sort: 'FullName',
          isDescending: false
        })
        this.users = data.items
      } catch (e) {
        this.handleError(e)
      }
    }
  },
  watch: {
    realm: {
      immediate: true,
      async handler() {
        await this.refresh()
      }
    }
  }
}
</script>
