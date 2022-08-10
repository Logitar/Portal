<template>
  <td class="d-flex">
    <template v-if="actor">
      <div class="d-flex">
        <div class="d-flex align-content-center flex-wrap mx-1">
          <b-link v-if="href" :href="href" target="_blank">
            <user-avatar v-if="user" :user="user" />
            <b-avatar v-else-if="icon" :variant="variant"><font-awesome-icon :icon="icon" /></b-avatar>
          </b-link>
          <template v-else>
            <user-avatar v-if="user" :user="user" />
            <b-avatar v-else-if="icon" :variant="variant"><font-awesome-icon :icon="icon" /></b-avatar>
          </template>
        </div>
      </div>
      <div>
        {{ $d(new Date(date), 'medium') }}
        <br />
        {{ $t('by') }}
        <b-link v-if="href" :href="href" target="_blank">{{ name }} <font-awesome-icon icon="external-link-alt" /></b-link>
        <template v-else>{{ name }}</template>
      </div>
    </template>
    <template v-else>{{ $d(new Date(date), 'medium') }}</template>
  </td>
</template>

<script>
import UserAvatar from '@/components/User/UserAvatar.vue'

export default {
  name: 'UpdatedCell',
  components: {
    UserAvatar
  },
  props: {
    actor: {
      type: Object,
      default: null
    },
    date: {
      type: String,
      required: true
    }
  },
  computed: {
    href() {
      const { id, isDeleted, type } = this.actor
      if (!isDeleted) {
        switch (type) {
          case 'ApiKey':
            return `/api-keys/${id}`
          case 'User':
            return `/users/${id}`
        }
      }
      return ''
    },
    icon() {
      switch (this.actor.type) {
        case 'ApiKey':
          return 'key'
        case 'System':
          return 'robot'
      }
      return ''
    },
    name() {
      const { name, type } = this.actor
      return type === 'System' ? this.$i18n.t('system') : name
    },
    user() {
      const { email, name, picture, type } = this.actor
      if (type !== 'User') {
        return null
      }
      return {
        email,
        picture,
        username: name
      }
    },
    variant() {
      return this.actor.type === 'ApiKey' ? 'info' : ''
    }
  }
}
</script>

<style scoped></style>
