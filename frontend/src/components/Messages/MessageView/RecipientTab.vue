<template>
  <b-tab :title="$t('messages.recipients')">
    <table class="table table-striped">
      <thead>
        <tr>
          <th scope="col" v-t="'messages.recipient.type.label'" />
          <th scope="col" v-t="'messages.recipient.address'" />
          <th scope="col" v-t="'messages.recipient.displayName'" />
          <th scope="col" v-t="'messages.recipient.user'" />
        </tr>
      </thead>
      <tbody>
        <tr v-for="(recipient, index) in message.recipients" :key="index">
          <td>{{ $t(`messages.recipient.type.options.${recipient.type}`) }}</td>
          <td v-text="recipient.address" />
          <td v-text="recipient.displayName || '—'" />
          <td v-if="recipient.userId">
            <template v-if="users[recipient.userId]">
              <b-link :href="`/users/${recipient.userId}`" target="_blank">
                <img
                  v-if="users[recipient.userId].picture"
                  :src="users[recipient.userId].picture"
                  :alt="`${users[recipient.userId].username}'s avatar`"
                  class="rounded-circle"
                  width="24"
                  height="24"
                />
                <v-gravatar v-else-if="users[recipient.userId].email" class="rounded-circle" :email="users[recipient.userId].email" :size="24" />
                {{ users[recipient.userId].username }}
                <font-awesome-icon icon="external-link-alt" />
              </b-link>
            </template>
            <template v-else>
              <font-awesome-icon icon="user-alt-slash" />
              {{ recipient.username }}
            </template>
          </td>
          <td v-else>&mdash;</td>
        </tr>
      </tbody>
    </table>
  </b-tab>
</template>

<script>
import Vue from 'vue'
import { getUsers } from '@/api/users'

export default {
  name: 'RecipientTab',
  props: {
    message: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      users: {}
    }
  },
  async created() {
    try {
      const { data } = await getUsers({ realm: this.message.realmId })
      data.items.forEach(user => Vue.set(this.users, user.id, user))
    } catch (e) {
      this.handleError(e)
    }
  }
}
</script>
